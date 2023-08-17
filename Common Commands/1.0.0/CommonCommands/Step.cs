//				Copyright 2023 Keysight Technologies, Inc.
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the
// Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software,
// and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using OpenTap;
using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace OpenTap.Plugins.PluginDevelopment
{
    #region Common Methods
    public static class CommonMethods
    {
        public static void OpenHelpLink(string helplink)
        {
            string packagesPrefix = AppDomain.CurrentDomain.BaseDirectory + "Packages";
            bool notWindows = !RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            string filePath = packagesPrefix + helplink;
            filePath = notWindows ? filePath.Replace(@"\", "/") : filePath;
            if (File.Exists(filePath)) { Log.CreateSource("HelpLink").Info("Opening HTML file from the location - " + filePath); Process p = notWindows ? Process.Start(new ProcessStartInfo { FileName = "/bin/bash", Arguments = $"-c \"xdg-open '{filePath}'\"", UseShellExecute = false }) : Process.Start(filePath); }
            else { Log.CreateSource("HelpLink").Error("Helplink file could not be found at location - " + filePath); }
        }
    }
    #endregion
    #region Common Instrument Commands
    #region CommonCommandsAad
    [Display("*AAD", Groups: new[] { "Keysight Instrument Plugins", "Common Commands" }, Description: "This command, in conjunction with the Address Set protocol, allows the controller to detect all address-configurable devices (that is, devices that implement this command) and assign an IEEE 488.1 address to each of those devices.")]
    public class CommonCommandsAad : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\CommonCommands\Docs\IEEE-488_Commands\_AAD.htm");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public ScpiInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response Properties
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public CommonCommandsAad()
        {
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            CommonCommands commonCommands = new CommonCommands(myInstrument);
            commonCommands.SetAad();
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region CommonCommandsCal
    [Display("*CAL", Groups: new[] { "Keysight Instrument Plugins", "Common Commands" }, Description: "The Calibration query causes a device to perform an internal self-calibration and generate a response that indicates whether or not the device completed the self-calibration without error.")]
    public class CommonCommandsCal : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\CommonCommands\Docs\IEEE-488_Commands\_CAL.htm");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public ScpiInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response Properties
        [Output]
        [Display(Group: "Query Response ", Name: "Cal", Description: "Returns the result of executing a calibration.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public int? _calQR { get; private set; }
        #endregion
        #region Result Checkbox
        [Display("Publish Results", Group: "Results", Description: "Enable to publish results", Order: 58.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool publishResults { get; set; }
        #endregion
        #region Verdict Properties
        public enum VerdictTestEnum { EqualTo, LessThan, GreaterThan, InBetween, Ignore };
        [Display(Group: "Numeric Limit Test", Name: "Select Test", Order: 61.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public VerdictTestEnum VerdictTest { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Value", Order: 61.2, Description: "The value allowed. If not equal, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.EqualTo, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public int ValueEqualTo { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Lower Limit Value", Order: 61.3, Description: "The minimum value allowed. If less, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.GreaterThan, VerdictTestEnum.InBetween, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public int LowerLimit { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Upper Limit Value", Order: 61.4, Description: "The maximum value allowed. If exceeded, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.LessThan, VerdictTestEnum.InBetween, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public int UpperLimit { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public CommonCommandsCal()
        {
            VerdictTest = VerdictTestEnum.Ignore;
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            UpgradeVerdict(Verdict.NotSet);
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            CommonCommands commonCommands = new CommonCommands(myInstrument);
            _calQR = commonCommands.GetCal();

            int? result = _calQR;

            if ((result > LowerLimit && result < UpperLimit) && VerdictTest == VerdictTestEnum.InBetween)
            { UpgradeVerdict(Verdict.Pass); }
            else if ((result == ValueEqualTo) && VerdictTest == VerdictTestEnum.EqualTo)
            { UpgradeVerdict(Verdict.Pass); }
            else if ((result >= LowerLimit) && VerdictTest == VerdictTestEnum.GreaterThan)
            { UpgradeVerdict(Verdict.Pass); }
            else if ((result <= UpperLimit) && VerdictTest == VerdictTestEnum.LessThan)
            { UpgradeVerdict(Verdict.Pass); }
            else if (VerdictTest == VerdictTestEnum.Ignore)
            { UpgradeVerdict(Verdict.Pass); }
            else
            { UpgradeVerdict(Verdict.Fail); }
            if (publishResults)
            {
                Results.Publish("Cal", new { Cal = (int)_calQR });
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region CommonCommandsCls
    [Display("*CLS", Groups: new[] { "Keysight Instrument Plugins", "Common Commands" }, Description: "This command clears the event registers in all register groups. This command also clears the Error queue.")]
    public class CommonCommandsCls : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\CommonCommands\Docs\IEEE-488_Commands\_CLS.htm");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public ScpiInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response Properties
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public CommonCommandsCls()
        {
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            CommonCommands commonCommands = new CommonCommands(myInstrument);
            commonCommands.SetCls();
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region CommonCommandsDdt
    [Display("*DDT", Groups: new[] { "Keysight Instrument Plugins", "Common Commands" }, Description: "The Define Device Trigger command stores a command sequence that is executed when a group execute trigger (GET), IEEE 488.1 interface message, or *TRG common command is received.")]
    public class CommonCommandsDdt : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\CommonCommands\Docs\IEEE-488_Commands\_DDT.htm");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public ScpiInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response Properties
        public enum CommandSyntaxes { @String, @Block }
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]
        [Display(Group: "Settings ", Name: "Command Syntax", Order: 21.2)]
        public CommandSyntaxes commandSyntax { get; set; }
        [Display(Group: "Command Parameter ", Name: "String", Description: "A command sequence that is executed when a group execute trigger (GET), IEEE 488.1 interface message, or *TRG common command is received.", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]
        [EnabledIf("commandSyntax", CommandSyntaxes.@String, HideIfDisabled = true)]
        public string _stringCPString { get; set; }
        [Display(Group: "Command Parameter ", Name: "Block_data", Description: "A command sequence that is executed when a group execute trigger (GET), IEEE 488.1 interface message, or *TRG common command is received.", Order: 30.2)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]
        [EnabledIf("commandSyntax", CommandSyntaxes.@Block, HideIfDisabled = true)]
        public string _block_dataCPBlock { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "Ddt", Description: "Returns the current value of the command sequence that is executed when a group execute trigger (GET), IEEE 488.1 interface message, or *TRG common command is received.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public byte[] _ddtQR { get; private set; }
        #endregion
        #region Result Checkbox
        [Display("Publish Results", Group: "Results", Description: "Enable to publish results", Order: 58.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool publishResults { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public CommonCommandsDdt()
        {
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            CommonCommands commonCommands = new CommonCommands(myInstrument);
            if (Action == SetAction.Command)
            {
                if (commandSyntax == CommandSyntaxes.@String)
                {
                    commonCommands.SetDdtString(_stringCPString);
                }
                if (commandSyntax == CommandSyntaxes.@Block)
                {
                    commonCommands.SetDdtBlock(_block_dataCPBlock);
                }

            }
            else
            {
                _ddtQR = commonCommands.GetDdt();
                if (publishResults)
                {
                    List<string> columnNames = new List<string>() { "Index", "Ddt" };
                    Results.PublishTable("Ddt", new List<string>(columnNames), Enumerable.Range(1, _ddtQR.Length).ToArray(), _ddtQR);
                }
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region CommonCommandsDlf
    [Display("*DLF", Groups: new[] { "Keysight Instrument Plugins", "Common Commands" }, Description: "The Disable Listener Function command causes a device to cease being a listener (change to L0 subset).")]
    public class CommonCommandsDlf : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\CommonCommands\Docs\IEEE-488_Commands\_DLF.htm");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public ScpiInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response Properties
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public CommonCommandsDlf()
        {
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            CommonCommands commonCommands = new CommonCommands(myInstrument);
            commonCommands.SetDlf();
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region CommonCommandsDmc
    [Display("*DMC", Groups: new[] { "Keysight Instrument Plugins", "Common Commands" }, Description: "The Define Macro command allows the programmer to assign a sequence of zero or more program message unit elements to a macro label.")]
    public class CommonCommandsDmc : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\CommonCommands\Docs\IEEE-488_Commands\_DMC.htm");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public ScpiInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response Properties
        public enum CommandSyntaxes { @Block, @String }
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]
        [Display(Group: "Settings ", Name: "Command Syntax", Order: 21.2)]
        public CommandSyntaxes commandSyntax { get; set; }
        [Display(Group: "Command Parameter ", Name: "String", Description: "The macro label", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]
        [EnabledIf("commandSyntax", CommandSyntaxes.@Block, HideIfDisabled = true)]
        public string _stringCPBlock { get; set; }
        [Display(Group: "Command Parameter ", Name: "Block_data", Description: "The macro definition", Order: 30.2)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]
        [EnabledIf("commandSyntax", CommandSyntaxes.@Block, HideIfDisabled = true)]
        public string _block_dataCPBlock { get; set; }
        [Display(Group: "Command Parameter ", Name: "String", Description: "The macro label", Order: 30.3)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]
        [EnabledIf("commandSyntax", CommandSyntaxes.@String, HideIfDisabled = true)]
        public string _stringCPString { get; set; }
        [Display(Group: "Command Parameter ", Name: "Macro_string", Description: "The macro definition", Order: 30.4)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]
        [EnabledIf("commandSyntax", CommandSyntaxes.@String, HideIfDisabled = true)]
        public string _macro_stringCPString { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public CommonCommandsDmc()
        {
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            CommonCommands commonCommands = new CommonCommands(myInstrument);
            if (commandSyntax == CommandSyntaxes.@Block)
            {
                commonCommands.SetDmcBlock(_stringCPBlock, _block_dataCPBlock);
            }
            if (commandSyntax == CommandSyntaxes.@String)
            {
                commonCommands.SetDmcString(_stringCPString, _macro_stringCPString);
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region CommonCommandsEmc
    [Display("*EMC", Groups: new[] { "Keysight Instrument Plugins", "Common Commands" }, Description: "The Enable Macro command enables and disables expansion of macros.")]
    public class CommonCommandsEmc : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\CommonCommands\Docs\IEEE-488_Commands\_EMC.htm");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public ScpiInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response Properties
        [Display(Group: "Command Parameter ", Name: "Integer", Description: "Controls whether macro expansion is enabled or disabled.", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public int? _integerCP { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "Integer", Description: "Returns whether macro expansion is enabled or disabled.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public int? _integerQR { get; private set; }
        #endregion
        #region Result Checkbox
        [Display("Publish Results", Group: "Results", Description: "Enable to publish results", Order: 58.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool publishResults { get; set; }
        #endregion
        #region Verdict Properties
        public enum VerdictTestEnum { EqualTo, LessThan, GreaterThan, InBetween, Ignore };
        [Display(Group: "Numeric Limit Test", Name: "Select Test", Order: 61.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public VerdictTestEnum VerdictTest { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Value", Order: 61.2, Description: "The value allowed. If not equal, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.EqualTo, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public int ValueEqualTo { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Lower Limit Value", Order: 61.3, Description: "The minimum value allowed. If less, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.GreaterThan, VerdictTestEnum.InBetween, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public int LowerLimit { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Upper Limit Value", Order: 61.4, Description: "The maximum value allowed. If exceeded, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.LessThan, VerdictTestEnum.InBetween, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public int UpperLimit { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public CommonCommandsEmc()
        {
            VerdictTest = VerdictTestEnum.Ignore;
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            UpgradeVerdict(Verdict.NotSet);
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            CommonCommands commonCommands = new CommonCommands(myInstrument);
            if (Action == SetAction.Command)
            {
                commonCommands.SetEmc(_integerCP);

            }
            else
            {
                _integerQR = commonCommands.GetEmc();

                int? result = _integerQR;

                if ((result > LowerLimit && result < UpperLimit) && VerdictTest == VerdictTestEnum.InBetween)
                { UpgradeVerdict(Verdict.Pass); }
                else if ((result == ValueEqualTo) && VerdictTest == VerdictTestEnum.EqualTo)
                { UpgradeVerdict(Verdict.Pass); }
                else if ((result >= LowerLimit) && VerdictTest == VerdictTestEnum.GreaterThan)
                { UpgradeVerdict(Verdict.Pass); }
                else if ((result <= UpperLimit) && VerdictTest == VerdictTestEnum.LessThan)
                { UpgradeVerdict(Verdict.Pass); }
                else if (VerdictTest == VerdictTestEnum.Ignore)
                { UpgradeVerdict(Verdict.Pass); }
                else
                { UpgradeVerdict(Verdict.Fail); }
                if (publishResults)
                {
                    Results.Publish("Integer", new { Integer = (int)_integerQR });
                }
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region CommonCommandsEse
    [Display("*ESE", Groups: new[] { "Keysight Instrument Plugins", "Common Commands" }, Description: "This command enables bits in the enable register for the Standard Event Register group.")]
    public class CommonCommandsEse : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\CommonCommands\Docs\IEEE-488_Commands\_ESE.htm");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public ScpiInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response Properties
        [Display(Group: "Command Parameter ", Name: "Enable_value", Description: "A decimal value which corresponds to the binary-weighted sum of the bits in the register.", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public int? _enable_valueCP { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "Enable_value", Description: "Returns a decimal value which corresponds to the binary-weighted sum of the bits in the register.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public int? _enable_valueQR { get; private set; }
        #endregion
        #region Result Checkbox
        [Display("Publish Results", Group: "Results", Description: "Enable to publish results", Order: 58.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool publishResults { get; set; }
        #endregion
        #region Verdict Properties
        public enum VerdictTestEnum { EqualTo, LessThan, GreaterThan, InBetween, Ignore };
        [Display(Group: "Numeric Limit Test", Name: "Select Test", Order: 61.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public VerdictTestEnum VerdictTest { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Value", Order: 61.2, Description: "The value allowed. If not equal, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.EqualTo, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public int ValueEqualTo { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Lower Limit Value", Order: 61.3, Description: "The minimum value allowed. If less, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.GreaterThan, VerdictTestEnum.InBetween, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public int LowerLimit { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Upper Limit Value", Order: 61.4, Description: "The maximum value allowed. If exceeded, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.LessThan, VerdictTestEnum.InBetween, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public int UpperLimit { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public CommonCommandsEse()
        {
            VerdictTest = VerdictTestEnum.Ignore;
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            UpgradeVerdict(Verdict.NotSet);
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            CommonCommands commonCommands = new CommonCommands(myInstrument);
            if (Action == SetAction.Command)
            {
                commonCommands.SetEse(_enable_valueCP);

            }
            else
            {
                _enable_valueQR = commonCommands.GetEse();

                int? result = _enable_valueQR;

                if ((result > LowerLimit && result < UpperLimit) && VerdictTest == VerdictTestEnum.InBetween)
                { UpgradeVerdict(Verdict.Pass); }
                else if ((result == ValueEqualTo) && VerdictTest == VerdictTestEnum.EqualTo)
                { UpgradeVerdict(Verdict.Pass); }
                else if ((result >= LowerLimit) && VerdictTest == VerdictTestEnum.GreaterThan)
                { UpgradeVerdict(Verdict.Pass); }
                else if ((result <= UpperLimit) && VerdictTest == VerdictTestEnum.LessThan)
                { UpgradeVerdict(Verdict.Pass); }
                else if (VerdictTest == VerdictTestEnum.Ignore)
                { UpgradeVerdict(Verdict.Pass); }
                else
                { UpgradeVerdict(Verdict.Fail); }
                if (publishResults)
                {
                    Results.Publish("Enable_value", new { Enable_value = (int)_enable_valueQR });
                }
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region CommonCommandsEsr
    [Display("*ESR", Groups: new[] { "Keysight Instrument Plugins", "Common Commands" }, Description: "This command queries the event register for the Standard Event Register group.")]
    public class CommonCommandsEsr : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\CommonCommands\Docs\IEEE-488_Commands\_ESR_.htm");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public ScpiInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response Properties
        [Output]
        [Display(Group: "Query Response ", Name: "Esr", Description: "Returns the current value of the event register for the Standard Event Register group.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public int? _esrQR { get; private set; }
        #endregion
        #region Result Checkbox
        [Display("Publish Results", Group: "Results", Description: "Enable to publish results", Order: 58.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool publishResults { get; set; }
        #endregion
        #region Verdict Properties
        public enum VerdictTestEnum { EqualTo, LessThan, GreaterThan, InBetween, Ignore };
        [Display(Group: "Numeric Limit Test", Name: "Select Test", Order: 61.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public VerdictTestEnum VerdictTest { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Value", Order: 61.2, Description: "The value allowed. If not equal, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.EqualTo, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public int ValueEqualTo { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Lower Limit Value", Order: 61.3, Description: "The minimum value allowed. If less, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.GreaterThan, VerdictTestEnum.InBetween, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public int LowerLimit { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Upper Limit Value", Order: 61.4, Description: "The maximum value allowed. If exceeded, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.LessThan, VerdictTestEnum.InBetween, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public int UpperLimit { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public CommonCommandsEsr()
        {
            VerdictTest = VerdictTestEnum.Ignore;
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            UpgradeVerdict(Verdict.NotSet);
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            CommonCommands commonCommands = new CommonCommands(myInstrument);
            _esrQR = commonCommands.GetEsr();

            int? result = _esrQR;

            if ((result > LowerLimit && result < UpperLimit) && VerdictTest == VerdictTestEnum.InBetween)
            { UpgradeVerdict(Verdict.Pass); }
            else if ((result == ValueEqualTo) && VerdictTest == VerdictTestEnum.EqualTo)
            { UpgradeVerdict(Verdict.Pass); }
            else if ((result >= LowerLimit) && VerdictTest == VerdictTestEnum.GreaterThan)
            { UpgradeVerdict(Verdict.Pass); }
            else if ((result <= UpperLimit) && VerdictTest == VerdictTestEnum.LessThan)
            { UpgradeVerdict(Verdict.Pass); }
            else if (VerdictTest == VerdictTestEnum.Ignore)
            { UpgradeVerdict(Verdict.Pass); }
            else
            { UpgradeVerdict(Verdict.Fail); }
            if (publishResults)
            {
                Results.Publish("Esr", new { Esr = (int)_esrQR });
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region CommonCommandsGmc
    [Display("*GMC", Groups: new[] { "Keysight Instrument Plugins", "Common Commands" }, Description: "The Get Macro Contents query allows the current definition of a macro to be retrieved from a device.")]
    public class CommonCommandsGmc : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\CommonCommands\Docs\IEEE-488_Commands\_GMC.htm");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public ScpiInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response Properties
        [Display(Group: "Query Parameter ", Name: "Label", Description: "The macro label.", Order: 40.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public string _labelQP { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "Gmc", Description: "Returns the macro definition for the specified macro label.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public byte[] _gmcQR { get; private set; }
        #endregion
        #region Result Checkbox
        [Display("Publish Results", Group: "Results", Description: "Enable to publish results", Order: 58.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool publishResults { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public CommonCommandsGmc()
        {
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            CommonCommands commonCommands = new CommonCommands(myInstrument);
            _gmcQR = commonCommands.GetGmc(_labelQP);
            if (publishResults)
            {
                List<string> columnNames = new List<string>() { "Index", "Gmc" };
                Results.PublishTable("Gmc", new List<string>(columnNames), Enumerable.Range(1, _gmcQR.Length).ToArray(), _gmcQR);
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region CommonCommandsIdn
    [Display("*IDN", Groups: new[] { "Keysight Instrument Plugins", "Common Commands" }, Description: "This command reads the instrument's (mainframe) identification string which contains comma-separated fields.")]
    public class CommonCommandsIdn : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\CommonCommands\Docs\IEEE-488_Commands\_IDN_.htm");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public ScpiInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response Properties
        [Output]
        [Display(Group: "Query Response ", Name: "Idn", Description: "Returns the current value of the instrument's identification string.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public string _idnQR { get; private set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public CommonCommandsIdn()
        {
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            CommonCommands commonCommands = new CommonCommands(myInstrument);
            _idnQR = commonCommands.GetIdn();
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region CommonCommandsIst
    [Display("*IST", Groups: new[] { "Keysight Instrument Plugins", "Common Commands" }, Description: "The Individual Status query allows the programmer to read the current state of the IEEE 488.1 defined “ist” local message in the device.")]
    public class CommonCommandsIst : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\CommonCommands\Docs\IEEE-488_Commands\_IST.htm");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public ScpiInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response Properties
        [Output]
        [Display(Group: "Query Response ", Name: "Ist", Description: "Returns whether the ist local message is TRUE or FALSE.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public int? _istQR { get; private set; }
        #endregion
        #region Result Checkbox
        [Display("Publish Results", Group: "Results", Description: "Enable to publish results", Order: 58.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool publishResults { get; set; }
        #endregion
        #region Verdict Properties
        public enum VerdictTestEnum { EqualTo, LessThan, GreaterThan, InBetween, Ignore };
        [Display(Group: "Numeric Limit Test", Name: "Select Test", Order: 61.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public VerdictTestEnum VerdictTest { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Value", Order: 61.2, Description: "The value allowed. If not equal, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.EqualTo, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public int ValueEqualTo { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Lower Limit Value", Order: 61.3, Description: "The minimum value allowed. If less, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.GreaterThan, VerdictTestEnum.InBetween, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public int LowerLimit { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Upper Limit Value", Order: 61.4, Description: "The maximum value allowed. If exceeded, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.LessThan, VerdictTestEnum.InBetween, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public int UpperLimit { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public CommonCommandsIst()
        {
            VerdictTest = VerdictTestEnum.Ignore;
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            UpgradeVerdict(Verdict.NotSet);
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            CommonCommands commonCommands = new CommonCommands(myInstrument);
            _istQR = commonCommands.GetIst();

            int? result = _istQR;

            if ((result > LowerLimit && result < UpperLimit) && VerdictTest == VerdictTestEnum.InBetween)
            { UpgradeVerdict(Verdict.Pass); }
            else if ((result == ValueEqualTo) && VerdictTest == VerdictTestEnum.EqualTo)
            { UpgradeVerdict(Verdict.Pass); }
            else if ((result >= LowerLimit) && VerdictTest == VerdictTestEnum.GreaterThan)
            { UpgradeVerdict(Verdict.Pass); }
            else if ((result <= UpperLimit) && VerdictTest == VerdictTestEnum.LessThan)
            { UpgradeVerdict(Verdict.Pass); }
            else if (VerdictTest == VerdictTestEnum.Ignore)
            { UpgradeVerdict(Verdict.Pass); }
            else
            { UpgradeVerdict(Verdict.Fail); }
            if (publishResults)
            {
                Results.Publish("Ist", new { Ist = (int)_istQR });
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region CommonCommandsLmc
    [Display("*LMC", Groups: new[] { "Keysight Instrument Plugins", "Common Commands" }, Description: "This query returns the currently defined macro labels.")]
    public class CommonCommandsLmc : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\CommonCommands\Docs\IEEE-488_Commands\_LMC.htm");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public ScpiInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response Properties
        [Display(Group: "Query Response ", Name: "Lmc File Path", Description: "Returns the set of currently defined macro labels.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public string _lmcQR { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public CommonCommandsLmc()
        {
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            CommonCommands commonCommands = new CommonCommands(myInstrument);
            string _arrayOutput = commonCommands.GetLmc();
            File.WriteAllText(_lmcQR, _arrayOutput);
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region CommonCommandsLrn
    [Display("*LRN", Groups: new[] { "Keysight Instrument Plugins", "Common Commands" }, Description: "This command returns the learn string, which is an ASCII string of SCPI commands.")]
    public class CommonCommandsLrn : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\CommonCommands\Docs\IEEE-488_Commands\_LRN_.htm");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public ScpiInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response Properties
        [Output]
        [Display(Group: "Query Response ", Name: "Lrn", Description: "Returns the learn string, which is an ASCII string of SCPI commands.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public string _lrnQR { get; private set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public CommonCommandsLrn()
        {
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            CommonCommands commonCommands = new CommonCommands(myInstrument);
            _lrnQR = commonCommands.GetLrn();
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region CommonCommandsOpc
    [Display("*OPC", Groups: new[] { "Keysight Instrument Plugins", "Common Commands" }, Description: "This command sets the ”Operation Complete” bit (bit 0) in the Standard Event register at the completion of the current operation.  This command returns ”1” to the output buffer at the completion of the current operation.")]
    public class CommonCommandsOpc : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\CommonCommands\Docs\IEEE-488_Commands\_OPC.htm");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public ScpiInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response Properties
        [Output]
        [Display(Group: "Query Response ", Name: "Opc", Description: "Returns the current value of  the ”Operation Complete” bit.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public int? _opcQR { get; private set; }
        #endregion
        #region Result Checkbox
        [Display("Publish Results", Group: "Results", Description: "Enable to publish results", Order: 58.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool publishResults { get; set; }
        #endregion
        #region Verdict Properties
        public enum VerdictTestEnum { EqualTo, LessThan, GreaterThan, InBetween, Ignore };
        [Display(Group: "Numeric Limit Test", Name: "Select Test", Order: 61.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public VerdictTestEnum VerdictTest { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Value", Order: 61.2, Description: "The value allowed. If not equal, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.EqualTo, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public int ValueEqualTo { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Lower Limit Value", Order: 61.3, Description: "The minimum value allowed. If less, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.GreaterThan, VerdictTestEnum.InBetween, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public int LowerLimit { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Upper Limit Value", Order: 61.4, Description: "The maximum value allowed. If exceeded, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.LessThan, VerdictTestEnum.InBetween, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public int UpperLimit { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public CommonCommandsOpc()
        {
            VerdictTest = VerdictTestEnum.Ignore;
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            UpgradeVerdict(Verdict.NotSet);
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            CommonCommands commonCommands = new CommonCommands(myInstrument);
            if (Action == SetAction.Command)
            {
                commonCommands.SetOpc();

            }
            else
            {
                _opcQR = commonCommands.GetOpc();

                int? result = _opcQR;

                if ((result > LowerLimit && result < UpperLimit) && VerdictTest == VerdictTestEnum.InBetween)
                { UpgradeVerdict(Verdict.Pass); }
                else if ((result == ValueEqualTo) && VerdictTest == VerdictTestEnum.EqualTo)
                { UpgradeVerdict(Verdict.Pass); }
                else if ((result >= LowerLimit) && VerdictTest == VerdictTestEnum.GreaterThan)
                { UpgradeVerdict(Verdict.Pass); }
                else if ((result <= UpperLimit) && VerdictTest == VerdictTestEnum.LessThan)
                { UpgradeVerdict(Verdict.Pass); }
                else if (VerdictTest == VerdictTestEnum.Ignore)
                { UpgradeVerdict(Verdict.Pass); }
                else
                { UpgradeVerdict(Verdict.Fail); }
                if (publishResults)
                {
                    Results.Publish("Opc", new { Opc = (int)_opcQR });
                }
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region CommonCommandsOpt
    [Display("*OPT", Groups: new[] { "Keysight Instrument Plugins", "Common Commands" }, Description: "The Option Identification query is for identifying reportable device options over the system interface.")]
    public class CommonCommandsOpt : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\CommonCommands\Docs\IEEE-488_Commands\_OPT.htm");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public ScpiInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response Properties
        [Output]
        [Display(Group: "Query Response ", Name: "Opt", Description: "Returns the instrument's reportable device options.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public string _optQR { get; private set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public CommonCommandsOpt()
        {
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            CommonCommands commonCommands = new CommonCommands(myInstrument);
            _optQR = commonCommands.GetOpt();
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region CommonCommandsPcb
    [Display("*PCB", Groups: new[] { "Keysight Instrument Plugins", "Common Commands" }, Description: "The Pass Control Back command is used by a controller to tell a device, being a potential controller, to which address the control is to be passed back when the device (acting as a controller) sends the IEEE 488.1 interface message, take control (TCT).")]
    public class CommonCommandsPcb : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\CommonCommands\Docs\IEEE-488_Commands\_PCB.htm");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public ScpiInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response Properties
        [Display(Group: "Command Parameter ", Name: "Integer1", Description: "The primary address of the controller sending the command.", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public int? _integer1CP { get; set; }
        [Display(Group: "Command Parameter ", Name: "Integer2", Description: "The secondary address of the controller sending the command.", Order: 30.2)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public int? _integer2CP { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public CommonCommandsPcb()
        {
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            CommonCommands commonCommands = new CommonCommands(myInstrument);
            commonCommands.SetPcb(_integer1CP, _integer2CP);
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region CommonCommandsPmc
    [Display("*PMC", Groups: new[] { "Keysight Instrument Plugins", "Common Commands" }, Description: "The Purge Macros command causes the device to delete all macros that may have been previously defined using the *DMC command.")]
    public class CommonCommandsPmc : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\CommonCommands\Docs\IEEE-488_Commands\_PMC.htm");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public ScpiInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response Properties
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public CommonCommandsPmc()
        {
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            CommonCommands commonCommands = new CommonCommands(myInstrument);
            commonCommands.SetPmc();
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region CommonCommandsPre
    [Display("*PRE", Groups: new[] { "Keysight Instrument Plugins", "Common Commands" }, Description: "The Parallel Poll Enable Register command sets the Parallel Poll Enable Register bits.  The Parallel Poll Enable Register query allows the programmer to determine the current contents of the Parallel Poll Enable Register.")]
    public class CommonCommandsPre : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\CommonCommands\Docs\IEEE-488_Commands\_PRE.htm");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public ScpiInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response Properties
        [Display(Group: "Command Parameter ", Name: "Integer", Description: "The Parallel Poll Enable Register.", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public int? _integerCP { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "Pre", Description: "Returns the current value of the Parallel Poll Enable Register.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public int? _preQR { get; private set; }
        #endregion
        #region Result Checkbox
        [Display("Publish Results", Group: "Results", Description: "Enable to publish results", Order: 58.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool publishResults { get; set; }
        #endregion
        #region Verdict Properties
        public enum VerdictTestEnum { EqualTo, LessThan, GreaterThan, InBetween, Ignore };
        [Display(Group: "Numeric Limit Test", Name: "Select Test", Order: 61.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public VerdictTestEnum VerdictTest { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Value", Order: 61.2, Description: "The value allowed. If not equal, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.EqualTo, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public int ValueEqualTo { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Lower Limit Value", Order: 61.3, Description: "The minimum value allowed. If less, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.GreaterThan, VerdictTestEnum.InBetween, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public int LowerLimit { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Upper Limit Value", Order: 61.4, Description: "The maximum value allowed. If exceeded, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.LessThan, VerdictTestEnum.InBetween, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public int UpperLimit { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public CommonCommandsPre()
        {
            VerdictTest = VerdictTestEnum.Ignore;
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            UpgradeVerdict(Verdict.NotSet);
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            CommonCommands commonCommands = new CommonCommands(myInstrument);
            if (Action == SetAction.Command)
            {
                commonCommands.SetPre(_integerCP);

            }
            else
            {
                _preQR = commonCommands.GetPre();

                int? result = _preQR;

                if ((result > LowerLimit && result < UpperLimit) && VerdictTest == VerdictTestEnum.InBetween)
                { UpgradeVerdict(Verdict.Pass); }
                else if ((result == ValueEqualTo) && VerdictTest == VerdictTestEnum.EqualTo)
                { UpgradeVerdict(Verdict.Pass); }
                else if ((result >= LowerLimit) && VerdictTest == VerdictTestEnum.GreaterThan)
                { UpgradeVerdict(Verdict.Pass); }
                else if ((result <= UpperLimit) && VerdictTest == VerdictTestEnum.LessThan)
                { UpgradeVerdict(Verdict.Pass); }
                else if (VerdictTest == VerdictTestEnum.Ignore)
                { UpgradeVerdict(Verdict.Pass); }
                else
                { UpgradeVerdict(Verdict.Fail); }
                if (publishResults)
                {
                    Results.Publish("Pre", new { Pre = (int)_preQR });
                }
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region CommonCommandsPsc
    [Display("*PSC", Groups: new[] { "Keysight Instrument Plugins", "Common Commands" }, Description: "This command enables or disables the clearing of certain enable registers at power on.")]
    public class CommonCommandsPsc : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\CommonCommands\Docs\IEEE-488_Commands\_PSC.htm");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public ScpiInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response Properties
        [Display(Group: "Command Parameter ", Name: "Psc", Description: "Controls the clearing of certain enable registers at power on.", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public int? _pscCP { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "Psc", Description: "Returns whether certain enable registers are cleared at power on.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public int? _pscQR { get; private set; }
        #endregion
        #region Result Checkbox
        [Display("Publish Results", Group: "Results", Description: "Enable to publish results", Order: 58.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool publishResults { get; set; }
        #endregion
        #region Verdict Properties
        public enum VerdictTestEnum { EqualTo, LessThan, GreaterThan, InBetween, Ignore };
        [Display(Group: "Numeric Limit Test", Name: "Select Test", Order: 61.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public VerdictTestEnum VerdictTest { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Value", Order: 61.2, Description: "The value allowed. If not equal, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.EqualTo, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public int ValueEqualTo { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Lower Limit Value", Order: 61.3, Description: "The minimum value allowed. If less, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.GreaterThan, VerdictTestEnum.InBetween, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public int LowerLimit { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Upper Limit Value", Order: 61.4, Description: "The maximum value allowed. If exceeded, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.LessThan, VerdictTestEnum.InBetween, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public int UpperLimit { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public CommonCommandsPsc()
        {
            VerdictTest = VerdictTestEnum.Ignore;
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            UpgradeVerdict(Verdict.NotSet);
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            CommonCommands commonCommands = new CommonCommands(myInstrument);
            if (Action == SetAction.Command)
            {
                commonCommands.SetPsc(_pscCP);

            }
            else
            {
                _pscQR = commonCommands.GetPsc();

                int? result = _pscQR;

                if ((result > LowerLimit && result < UpperLimit) && VerdictTest == VerdictTestEnum.InBetween)
                { UpgradeVerdict(Verdict.Pass); }
                else if ((result == ValueEqualTo) && VerdictTest == VerdictTestEnum.EqualTo)
                { UpgradeVerdict(Verdict.Pass); }
                else if ((result >= LowerLimit) && VerdictTest == VerdictTestEnum.GreaterThan)
                { UpgradeVerdict(Verdict.Pass); }
                else if ((result <= UpperLimit) && VerdictTest == VerdictTestEnum.LessThan)
                { UpgradeVerdict(Verdict.Pass); }
                else if (VerdictTest == VerdictTestEnum.Ignore)
                { UpgradeVerdict(Verdict.Pass); }
                else
                { UpgradeVerdict(Verdict.Fail); }
                if (publishResults)
                {
                    Results.Publish("Psc", new { Psc = (int)_pscQR });
                }
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region CommonCommandsPud
    [Display("*PUD", Groups: new[] { "Keysight Instrument Plugins", "Common Commands" }, Description: "The Protected User Data command stores data unique to the device such as calibration date, usage time, environmental conditions, and inventory control numbers.  The Protected User Data query allows the programmer to retrieve the contents of the *PUD storage area.")]
    public class CommonCommandsPud : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\CommonCommands\Docs\IEEE-488_Commands\_PUD.htm");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public ScpiInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response Properties
        public enum CommandSyntaxes { @String, @Block }
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]
        [Display(Group: "Settings ", Name: "Command Syntax", Order: 21.2)]
        public CommandSyntaxes commandSyntax { get; set; }
        [Display(Group: "Command Parameter ", Name: "String", Description: "Instrument data.", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]
        [EnabledIf("commandSyntax", CommandSyntaxes.@String, HideIfDisabled = true)]
        public string _stringCPString { get; set; }
        [Display(Group: "Command Parameter ", Name: "Block_data", Description: "Instrument data.", Order: 30.2)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]
        [EnabledIf("commandSyntax", CommandSyntaxes.@Block, HideIfDisabled = true)]
        public byte[] _block_dataCPBlock { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "Pud", Description: "Returns the current value of the instrument data.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public byte[] _pudQR { get; private set; }
        #endregion
        #region Result Checkbox
        [Display("Publish Results", Group: "Results", Description: "Enable to publish results", Order: 58.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool publishResults { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public CommonCommandsPud()
        {
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            CommonCommands commonCommands = new CommonCommands(myInstrument);
            if (Action == SetAction.Command)
            {
                if (commandSyntax == CommandSyntaxes.@String)
                {
                    commonCommands.SetPudString(_stringCPString);
                }
                if (commandSyntax == CommandSyntaxes.@Block)
                {
                    commonCommands.SetPudBlock(_block_dataCPBlock);
                }

            }
            else
            {
                _pudQR = commonCommands.GetPud();
                if (publishResults)
                {
                    List<string> columnNames = new List<string>() { "Index", "Pud" };
                    Results.PublishTable("Pud", new List<string>(columnNames), Enumerable.Range(1, _pudQR.Length).ToArray(), _pudQR);
                }
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region CommonCommandsRcl
    [Display("*RCL", Groups: new[] { "Keysight Instrument Plugins", "Common Commands" }, Description: "This command recalls the instrument state stored in the specified storage location.")]
    public class CommonCommandsRcl : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\CommonCommands\Docs\IEEE-488_Commands\_RCL.htm");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public ScpiInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response Properties
        [Display(Group: "Command Parameter ", Name: "Rcl", Description: "The instrument state register identifier.", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public int? _rclCP { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public CommonCommandsRcl()
        {
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            CommonCommands commonCommands = new CommonCommands(myInstrument);
            commonCommands.SetRcl(_rclCP);
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region CommonCommandsRdt
    [Display("*RDT", Groups: new[] { "Keysight Instrument Plugins", "Common Commands" }, Description: "The Resource Description Transfer command allows a Resource Description to be stored in a device.  The Resource Description Transfer query allows a Resource Description to be retrieved from a device. The Resource Description may be memory or in a read-write memory settable by the *RDT command.")]
    public class CommonCommandsRdt : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\CommonCommands\Docs\IEEE-488_Commands\_RDT.htm");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public ScpiInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response Properties
        public enum CommandSyntaxes { @String, @Block }
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]
        [Display(Group: "Settings ", Name: "Command Syntax", Order: 21.2)]
        public CommandSyntaxes commandSyntax { get; set; }
        [Display(Group: "Command Parameter ", Name: "String", Description: "The Resource Description.", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]
        [EnabledIf("commandSyntax", CommandSyntaxes.@String, HideIfDisabled = true)]
        public string _stringCPString { get; set; }
        [Display(Group: "Command Parameter ", Name: "Block_data", Description: "The Resource Description.", Order: 30.2)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]
        [EnabledIf("commandSyntax", CommandSyntaxes.@Block, HideIfDisabled = true)]
        public byte[] _block_dataCPBlock { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "Rdt", Description: "Returns the current value of the The Resource Description.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public byte[] _rdtQR { get; private set; }
        #endregion
        #region Result Checkbox
        [Display("Publish Results", Group: "Results", Description: "Enable to publish results", Order: 58.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool publishResults { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public CommonCommandsRdt()
        {
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            CommonCommands commonCommands = new CommonCommands(myInstrument);
            if (Action == SetAction.Command)
            {
                if (commandSyntax == CommandSyntaxes.@String)
                {
                    commonCommands.SetRdtString(_stringCPString);
                }
                if (commandSyntax == CommandSyntaxes.@Block)
                {
                    commonCommands.SetRdtBlock(_block_dataCPBlock);
                }

            }
            else
            {
                _rdtQR = commonCommands.GetRdt();
                if (publishResults)
                {
                    List<string> columnNames = new List<string>() { "Index", "Rdt" };
                    Results.PublishTable("Rdt", new List<string>(columnNames), Enumerable.Range(1, _rdtQR.Length).ToArray(), _rdtQR);
                }
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region CommonCommandsRst
    [Display("*RST", Groups: new[] { "Keysight Instrument Plugins", "Common Commands" }, Description: "This command resets the multimeter to the Factory configuration.")]
    public class CommonCommandsRst : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\CommonCommands\Docs\IEEE-488_Commands\_RST.htm");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public ScpiInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response Properties
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public CommonCommandsRst()
        {
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            CommonCommands commonCommands = new CommonCommands(myInstrument);
            commonCommands.SetRst();
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region CommonCommandsSav
    [Display("*SAV", Groups: new[] { "Keysight Instrument Plugins", "Common Commands" }, Description: "This command stores (saves) the current instrument state in the specified storage location.")]
    public class CommonCommandsSav : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\CommonCommands\Docs\IEEE-488_Commands\_SAV.htm");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public ScpiInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response Properties
        [Display(Group: "Command Parameter ", Name: "Sav", Description: "The instrument state register identifier.", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public int? _savCP { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public CommonCommandsSav()
        {
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            CommonCommands commonCommands = new CommonCommands(myInstrument);
            commonCommands.SetSav(_savCP);
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region CommonCommandsSre
    [Display("*SRE", Groups: new[] { "Keysight Instrument Plugins", "Common Commands" }, Description: "This command enables bits in the enable register for the Status Byte Register group.")]
    public class CommonCommandsSre : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\CommonCommands\Docs\IEEE-488_Commands\_SRE.htm");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public ScpiInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response Properties
        [Display(Group: "Command Parameter ", Name: "Enable_value", Description: "A decimal value which corresponds to the binary-weighted sum of the bits in the register.", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public int? _enable_valueCP { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "Enable_value", Description: "Returns the current value of the binary-weighted sum of the bits in the register.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public int? _enable_valueQR { get; private set; }
        #endregion
        #region Result Checkbox
        [Display("Publish Results", Group: "Results", Description: "Enable to publish results", Order: 58.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool publishResults { get; set; }
        #endregion
        #region Verdict Properties
        public enum VerdictTestEnum { EqualTo, LessThan, GreaterThan, InBetween, Ignore };
        [Display(Group: "Numeric Limit Test", Name: "Select Test", Order: 61.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public VerdictTestEnum VerdictTest { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Value", Order: 61.2, Description: "The value allowed. If not equal, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.EqualTo, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public int ValueEqualTo { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Lower Limit Value", Order: 61.3, Description: "The minimum value allowed. If less, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.GreaterThan, VerdictTestEnum.InBetween, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public int LowerLimit { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Upper Limit Value", Order: 61.4, Description: "The maximum value allowed. If exceeded, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.LessThan, VerdictTestEnum.InBetween, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public int UpperLimit { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public CommonCommandsSre()
        {
            VerdictTest = VerdictTestEnum.Ignore;
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            UpgradeVerdict(Verdict.NotSet);
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            CommonCommands commonCommands = new CommonCommands(myInstrument);
            if (Action == SetAction.Command)
            {
                commonCommands.SetSre(_enable_valueCP);

            }
            else
            {
                _enable_valueQR = commonCommands.GetSre();

                int? result = _enable_valueQR;

                if ((result > LowerLimit && result < UpperLimit) && VerdictTest == VerdictTestEnum.InBetween)
                { UpgradeVerdict(Verdict.Pass); }
                else if ((result == ValueEqualTo) && VerdictTest == VerdictTestEnum.EqualTo)
                { UpgradeVerdict(Verdict.Pass); }
                else if ((result >= LowerLimit) && VerdictTest == VerdictTestEnum.GreaterThan)
                { UpgradeVerdict(Verdict.Pass); }
                else if ((result <= UpperLimit) && VerdictTest == VerdictTestEnum.LessThan)
                { UpgradeVerdict(Verdict.Pass); }
                else if (VerdictTest == VerdictTestEnum.Ignore)
                { UpgradeVerdict(Verdict.Pass); }
                else
                { UpgradeVerdict(Verdict.Fail); }
                if (publishResults)
                {
                    Results.Publish("Enable_value", new { Enable_value = (int)_enable_valueQR });
                }
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region CommonCommandsStb
    [Display("*STB", Groups: new[] { "Keysight Instrument Plugins", "Common Commands" }, Description: "This command queries the condition register for the Status Byte Register group.")]
    public class CommonCommandsStb : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\CommonCommands\Docs\IEEE-488_Commands\_STB_.htm");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public ScpiInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response Properties
        [Output]
        [Display(Group: "Query Response ", Name: "Stb", Description: "Returns a decimal value which corresponds to the binary-weighted sum of all bits set in the register", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public int? _stbQR { get; private set; }
        #endregion
        #region Result Checkbox
        [Display("Publish Results", Group: "Results", Description: "Enable to publish results", Order: 58.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool publishResults { get; set; }
        #endregion
        #region Verdict Properties
        public enum VerdictTestEnum { EqualTo, LessThan, GreaterThan, InBetween, Ignore };
        [Display(Group: "Numeric Limit Test", Name: "Select Test", Order: 61.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public VerdictTestEnum VerdictTest { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Value", Order: 61.2, Description: "The value allowed. If not equal, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.EqualTo, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public int ValueEqualTo { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Lower Limit Value", Order: 61.3, Description: "The minimum value allowed. If less, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.GreaterThan, VerdictTestEnum.InBetween, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public int LowerLimit { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Upper Limit Value", Order: 61.4, Description: "The maximum value allowed. If exceeded, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.LessThan, VerdictTestEnum.InBetween, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public int UpperLimit { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public CommonCommandsStb()
        {
            VerdictTest = VerdictTestEnum.Ignore;
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            UpgradeVerdict(Verdict.NotSet);
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            CommonCommands commonCommands = new CommonCommands(myInstrument);
            _stbQR = commonCommands.GetStb();

            int? result = _stbQR;

            if ((result > LowerLimit && result < UpperLimit) && VerdictTest == VerdictTestEnum.InBetween)
            { UpgradeVerdict(Verdict.Pass); }
            else if ((result == ValueEqualTo) && VerdictTest == VerdictTestEnum.EqualTo)
            { UpgradeVerdict(Verdict.Pass); }
            else if ((result >= LowerLimit) && VerdictTest == VerdictTestEnum.GreaterThan)
            { UpgradeVerdict(Verdict.Pass); }
            else if ((result <= UpperLimit) && VerdictTest == VerdictTestEnum.LessThan)
            { UpgradeVerdict(Verdict.Pass); }
            else if (VerdictTest == VerdictTestEnum.Ignore)
            { UpgradeVerdict(Verdict.Pass); }
            else
            { UpgradeVerdict(Verdict.Fail); }
            if (publishResults)
            {
                Results.Publish("Stb", new { Stb = (int)_stbQR });
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region CommonCommandsTrg
    [Display("*TRG", Groups: new[] { "Keysight Instrument Plugins", "Common Commands" }, Description: "This command is used to trigger the instrument from the remote interface.")]
    public class CommonCommandsTrg : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\CommonCommands\Docs\IEEE-488_Commands\_TRG.htm");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public ScpiInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response Properties
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public CommonCommandsTrg()
        {
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            CommonCommands commonCommands = new CommonCommands(myInstrument);
            commonCommands.SetTrg();
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region CommonCommandsTst
    [Display("*TST", Groups: new[] { "Keysight Instrument Plugins", "Common Commands" }, Description: "This command performs a complete self-test of the instrument and returns a pass/fail indication.")]
    public class CommonCommandsTst : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\CommonCommands\Docs\IEEE-488_Commands\_TST_.htm");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public ScpiInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response Properties
        [Output]
        [Display(Group: "Query Response ", Name: "Tst", Description: "The command returns ”+0” (all tests passed) or ”+1” (one or more tests failed).", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public int? _tstQR { get; private set; }
        #endregion
        #region Result Checkbox
        [Display("Publish Results", Group: "Results", Description: "Enable to publish results", Order: 58.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool publishResults { get; set; }
        #endregion
        #region Verdict Properties
        public enum VerdictTestEnum { EqualTo, LessThan, GreaterThan, InBetween, Ignore };
        [Display(Group: "Numeric Limit Test", Name: "Select Test", Order: 61.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public VerdictTestEnum VerdictTest { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Value", Order: 61.2, Description: "The value allowed. If not equal, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.EqualTo, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public int ValueEqualTo { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Lower Limit Value", Order: 61.3, Description: "The minimum value allowed. If less, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.GreaterThan, VerdictTestEnum.InBetween, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public int LowerLimit { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Upper Limit Value", Order: 61.4, Description: "The maximum value allowed. If exceeded, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.LessThan, VerdictTestEnum.InBetween, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public int UpperLimit { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public CommonCommandsTst()
        {
            VerdictTest = VerdictTestEnum.Ignore;
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            UpgradeVerdict(Verdict.NotSet);
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            CommonCommands commonCommands = new CommonCommands(myInstrument);
            _tstQR = commonCommands.GetTst();

            int? result = _tstQR;

            if ((result > LowerLimit && result < UpperLimit) && VerdictTest == VerdictTestEnum.InBetween)
            { UpgradeVerdict(Verdict.Pass); }
            else if ((result == ValueEqualTo) && VerdictTest == VerdictTestEnum.EqualTo)
            { UpgradeVerdict(Verdict.Pass); }
            else if ((result >= LowerLimit) && VerdictTest == VerdictTestEnum.GreaterThan)
            { UpgradeVerdict(Verdict.Pass); }
            else if ((result <= UpperLimit) && VerdictTest == VerdictTestEnum.LessThan)
            { UpgradeVerdict(Verdict.Pass); }
            else if (VerdictTest == VerdictTestEnum.Ignore)
            { UpgradeVerdict(Verdict.Pass); }
            else
            { UpgradeVerdict(Verdict.Fail); }
            if (publishResults)
            {
                Results.Publish("Tst", new { Tst = (int)_tstQR });
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region CommonCommandsWai
    [Display("*WAI", Groups: new[] { "Keysight Instrument Plugins", "Common Commands" }, Description: "This command configures the instrument's output buffer to wait for all pending operations to complete before executing any additional commands over the interface.")]
    public class CommonCommandsWai : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\CommonCommands\Docs\IEEE-488_Commands\_WAI.htm");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public ScpiInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response Properties
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public CommonCommandsWai()
        {
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            CommonCommands commonCommands = new CommonCommands(myInstrument);
            commonCommands.SetWai();
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region CommonCommandsRmc
    [Display("*RMC", Groups: new[] { "Keysight Instrument Plugins", "Common Commands" }, Description: "The Remove Individual Macro command removes a single macro definition from the device.")]
    public class CommonCommandsRmc : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\CommonCommands\Docs\IEEE-488_Commands\_RMC.htm");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public ScpiInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response Properties
        [Display(Group: "Command Parameter ", Name: "Label", Description: "The label of the macro to be removed.", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public string _labelCP { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public CommonCommandsRmc()
        {
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            CommonCommands commonCommands = new CommonCommands(myInstrument);
            commonCommands.SetRmc(_labelCP);
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region CommonCommandsSds
    [Display("*SDS", Groups: new[] { "Keysight Instrument Plugins", "Common Commands" }, Description: "The Save Default Device Settings command initializes the contents of a save/recall register.")]
    public class CommonCommandsSds : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\CommonCommands\Docs\IEEE-488_Commands\_SDS.htm");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public ScpiInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response Properties
        [Display(Group: "Command Parameter ", Name: "Integer", Description: "The save/recall register identifier.", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public int? _integerCP { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public CommonCommandsSds()
        {
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            CommonCommands commonCommands = new CommonCommands(myInstrument);
            commonCommands.SetSds(_integerCP);
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #endregion
}