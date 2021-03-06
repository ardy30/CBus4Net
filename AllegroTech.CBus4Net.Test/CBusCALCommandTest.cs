﻿using System;
using System.Collections.Generic;

using Xunit;

namespace AllegroTech.CBus4Net.Test
{
    /// <summary>
    /// There are still more CBus addressing issues to resolve
    /// For bridging etc....
    /// </summary>    
    [Trait("CBus", "CAL Commands")]
    public class CBusCALCommandTest
    {
        private static void ValidateCALMessage(Protocol.CBusCALCommand cmd, string cmdString)
        {
            var expectedMessageLength = 11;
            if (!cmd.IncludeChecksum)
                expectedMessageLength--;

            // "Unexpected CBus CAL command length, check hex number formatting?");
            Assert.Equal(
                expectedMessageLength, 
                cmdString.Length);
        }


        [Fact]
        public void Generate_CAL_ResetCommand()
        {
            Console.WriteLine(
             Protocol.CBusCALCommandBuilder.Build_ResetCommand()
             );
        }

        [Fact]
        public void Generate_CAL_Options1_Command()
        {
            var cmd = Protocol.CBusCALCommandBuilder.Build_SetCAL_Options1(Protocol.CBusProtcol.Interface_Options_1.CONNECT);

            Console.WriteLine(cmd);

            var cmdString = cmd.ToCBusString();
            Console.WriteLine(cmd.ToCBusString());

            ValidateCALMessage(cmd, cmdString);

        }


        [Fact]
        public void Generate_CAL_Application1_Command()
        {
            var cmd = Protocol.CBusCALCommandBuilder.Build_RegisterApplication1Monitor(Protocol.CBusProtcol.CBusApplicationId.TRIGGER);

            Console.WriteLine(cmd);

            var cmdString = cmd.ToCBusString();
            Console.WriteLine(cmd.ToCBusString());
            ValidateCALMessage(cmd, cmdString);
        }

        [Fact]
        public void Generate_CAL_Application2_Command()
        {
            var cmd = Protocol.CBusCALCommandBuilder.Build_RegisterApplication2Monitor(Protocol.CBusProtcol.CBusApplicationId.TRIGGER);

            Console.WriteLine(cmd);

            var cmdString = cmd.ToCBusString();
            Console.WriteLine(cmd.ToCBusString());
            ValidateCALMessage(cmd, cmdString);
        }
    }
}