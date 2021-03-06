﻿using System;
using System.Collections.Generic;

using Xunit;
using Xunit.Abstractions;

namespace AllegroTech.CBus4Net.Test
{
    [Trait("CBus", "Trigger")]
    public class CBusTriggerTest
    {
        private readonly ITestOutputHelper Output;

        public CBusTriggerTest(ITestOutputHelper output)
        {
            this.Output = output;
        }

        #region TRIGGER COMAMNDS
        [Fact(DisplayName="Test_That_Reference_TriggerCommand_Can_Be_Parsed_Correctly")]
        public void Test_That_Reference_TriggerCommand_Can_Be_Parsed_Correctly()
        {
            // Taken from 'C-Bus Quick Start Guide.pdf' Page 7
            var referenceMessageData = "05CA0002250109\r";
            Output.WriteLine("Taken from 'C-Bus Quick Start Guide.pdf' Page 7: {0}", referenceMessageData);
            

            var CBusAddressMap = new Protocol.CBusApplicationAddressMap();
            CBusAddressMap.AddMapping(Protocol.CBusProtcol.ApplicationTypes.LIGHTING, 0x38);
            CBusAddressMap.AddMapping(Protocol.CBusProtcol.ApplicationTypes.TRIGGER, 0xCA);

            var cbusProtocol = new Protocol.CBusProtcol(128);

            var _state = new Protocol.CBusProtcol.CBusStateMachine();
            var data = referenceMessageData.ToCharArray();
            int dataIndex = 0;

            
            Assert.True(
                    cbusProtocol.TryProcessReceivedBytes(
                        data, data.Length,
                        ref dataIndex, _state
                        ),

                    "Failed to parse reference lighting command"
                    );
        }

        [Fact(DisplayName="Test_That_Reference_TriggerCommand_Can_Be_Interpreted_Correctly")]
        public void Test_That_Reference_TriggerCommand_Can_Be_Interpreted_Correctly()
        {
            Output.WriteLine("Taken from 'Trigger Control Application.pdf' Page 14");
            // Taken from 'Trigger Control Application.pdf' Page 14
            byte[] referenceMessageData = { 0x05, 0xCA, 0x00, 0x02, 0x25, 0x01, 0x09 };

            var CBusAddressMap = new Protocol.CBusApplicationAddressMap();
            CBusAddressMap.AddMapping(Protocol.CBusProtcol.ApplicationTypes.LIGHTING, 0x38);
            CBusAddressMap.AddMapping(Protocol.CBusProtcol.ApplicationTypes.TRIGGER, 0xCA);

            Protocol.CBusSALCommand cmd;
            Assert.True(
                CBusAddressMap.TryParseCommand(referenceMessageData, referenceMessageData.Length, false, false, out cmd),
                "Failed to interpret reference lighting command"
                );

            Assert.True(
                cmd.ApplicationType == Protocol.CBusProtcol.ApplicationTypes.TRIGGER,
                "Expected Lighting Command Type"
                );
        }
        [Fact(DisplayName="Test_That_Reference_TriggerCommand_Can_Be_Parsed_And_The_Result_Interpreted_Correctly")]
        public void Test_That_Reference_TriggerCommand_Can_Be_Parsed_And_The_Result_Interpreted_Correctly()
        {
            //Take from 'C-Bus Quick Start Guide.pdf' Page 7
            var referenceMessageData = "05CA0002250109\r";
            //var referenceMessageData = "0505CA00020000\r";

            var CBusAddressMap = new Protocol.CBusApplicationAddressMap();
            CBusAddressMap.AddMapping(Protocol.CBusProtcol.ApplicationTypes.LIGHTING, 0x38);
            CBusAddressMap.AddMapping(Protocol.CBusProtcol.ApplicationTypes.TRIGGER, 0xCA);

            var cbusProtocol = new Protocol.CBusProtcol(128);

            var state = new Protocol.CBusProtcol.CBusStateMachine();
            var data = referenceMessageData.ToCharArray();
            int dataIndex = 0;
            Assert.True(
                    cbusProtocol.TryProcessReceivedBytes(
                        data, data.Length,
                        ref dataIndex, state
                        ),

                    "Failed to parse reference trigger command"
                    );

            var isMonitoredSAL = (state.MessageType == Protocol.CBusProtcol.CBusMessageType.MONITORED_SAL_MESSAGE_RECEIVED);
            Protocol.CBusSALCommand cmd;
            Assert.True(
                CBusAddressMap.TryParseCommand(state.CommandBytes, state.CommandLength, isMonitoredSAL ,true, out cmd),
                "Failed to interpret reference trigger command"
                );

            // Expected Trigger Command Type
            Assert.Equal(
                Protocol.CBusProtcol.ApplicationTypes.TRIGGER, 
                cmd.ApplicationType);

            Console.WriteLine("Parsed Command:{0}", cmd);
            foreach (var c in ((Protocol.CBusTriggerCommand)cmd).Commands())
            {
                Console.WriteLine("Child Command: **{0}**", c);
            }
        }

        #endregion
        [Fact(DisplayName="Test_That_Command_Can_Be_Processed_When_Recieved_In_Multiple_Parts_From_The_Network")]
        public void Test_That_Command_Can_Be_Processed_When_Recieved_In_Multiple_Parts_From_The_Network()
        {
            //string referenceMessageData = "056438007922C4\r\n";
            string referenceMessageData1 = "05643800";
            string referenceMessageData2 = "7922C4\r\n";

            var CBusAddressMap = new Protocol.CBusApplicationAddressMap();
            CBusAddressMap.AddMapping(Protocol.CBusProtcol.ApplicationTypes.LIGHTING, 0x38);
            CBusAddressMap.AddMapping(Protocol.CBusProtcol.ApplicationTypes.TRIGGER, 0xCA);

            var cbusProtocol = new Protocol.CBusProtcol(128);
 
            var state = new Protocol.CBusProtcol.CBusStateMachine();
            var data = referenceMessageData1.ToCharArray();
            int dataIndex = 0;
            Assert.False(
                    cbusProtocol.TryProcessReceivedBytes(
                        data, data.Length,
                        ref dataIndex, state
                        ),


                "should not reuturn success when only partial command received"
                );

            //Process another segment of data

            data = referenceMessageData2.ToCharArray();
            dataIndex = 0;
            Assert.True(
                    cbusProtocol.TryProcessReceivedBytes(
                        data, data.Length,
                        ref dataIndex, state
                        ),

                    "Failed to parse reference lighting command"
                    );

            var isMonitoredSAL = (state.MessageType == Protocol.CBusProtcol.CBusMessageType.MONITORED_SAL_MESSAGE_RECEIVED);
            Protocol.CBusSALCommand cmd;
            Assert.True(
                CBusAddressMap.TryParseCommand(state.CommandBytes, state.CommandLength, isMonitoredSAL, false, out cmd),
                "Failed to interpret reference lighting command"
                );

            // Expected lighting Command Type
            Assert.Equal<Protocol.CBusProtcol.ApplicationTypes>(
                Protocol.CBusProtcol.ApplicationTypes.LIGHTING, 
                cmd.ApplicationType);
                

            Console.WriteLine("Parsed Command:{0}", cmd);
            foreach (var c in ((Protocol.CBusLightingCommand)cmd).Commands())
            {
                Console.WriteLine("Child Command: **{0}**", c);
            }

        }


        [Theory(DisplayName="Test_Randomly_Selected_TriggerCommands_Can_Be_Parsed_And_The_Result_Interpreted_Correctly")]
        [InlineData("0505CA00020000\r\n")]
        [InlineData("0505CA00020002\r\n")]
        public void Test_Randomly_Selected_TriggerCommands_Can_Be_Parsed_And_The_Result_Interpreted_Correctly(string referenceMessageData)
        {
            
            var CBusAddressMap = new Protocol.CBusApplicationAddressMap();
            CBusAddressMap.AddMapping(Protocol.CBusProtcol.ApplicationTypes.LIGHTING, 0x38);
            CBusAddressMap.AddMapping(Protocol.CBusProtcol.ApplicationTypes.TRIGGER, 0xCA);

            var cbusProtocol = new Protocol.CBusProtcol(128);

            var state = new Protocol.CBusProtcol.CBusStateMachine();
            var data = referenceMessageData.ToCharArray();
            int dataIndex = 0;
            Assert.True(
                    cbusProtocol.TryProcessReceivedBytes(
                        data, data.Length,
                        ref dataIndex, state
                        ),

                    "Failed to parse reference trigger command"
                    );

            var isMonitoredSAL = (state.MessageType == Protocol.CBusProtcol.CBusMessageType.MONITORED_SAL_MESSAGE_RECEIVED);
            Protocol.CBusSALCommand cmd;
            Assert.True(
                CBusAddressMap.TryParseCommand(state.CommandBytes, state.CommandLength, isMonitoredSAL, false, out cmd),
                "Failed to interpret reference trigger command"
                );

            // Expected Trigger Command Type
            Assert.Equal(
                Protocol.CBusProtcol.ApplicationTypes.TRIGGER, 
                cmd.ApplicationType);

            Console.WriteLine("Parsed Command:{0}", cmd);
            foreach (var c in ((Protocol.CBusTriggerCommand)cmd).Commands())
            {
                Console.WriteLine("Child Command: **{0}**", c);
            }
        }



    }
}
