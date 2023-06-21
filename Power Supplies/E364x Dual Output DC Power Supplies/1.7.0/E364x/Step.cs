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
    #region E364xCommonCommandCls
    [Display("*CLS", Groups: new[] { "E364x" }, Description: "Clear all event registers and Status Byte register.")]
    public class E364xCommonCommandCls : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_84.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response Properties
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xCommonCommandCls()
        {
            {
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            myInstrument.CommonCommands.SetCls();
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xCommonCommandEse
    [Display("*ESE", Groups: new[] { "E364x" }, Description: "Query the Standard Event Enable register.   Enable bits in the Standard Event Enable register.")]
    public class E364xCommonCommandEse : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_86.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response Properties
        [Display(Group: "Command Parameter ", Name: "enableValue", Description: "The Standard Event Enable register.", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public int? _enableValueCP { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "enableValue", Description: "Returns the current value of the Standard Event Enable register.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public int? _enableValueQR { get; private set; }
        #endregion
        #region Result Checkbox
        [Display("Publish Results", Group: "Results", Description: "Enable to publish results", Order: 58.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool publishResults { get; set; }
        #endregion
        #region Verdict Properties
        [Output]
        [Display("Verdict", Group: "Verdict", Order: 60.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public Verdict MyVerdict { get; private set; }
        public enum VerdictTestEnum { EqualTo, LessThan, GreaterThan, InBetween, Ignore };
        [Display(Group: "Numeric Limit Test", Name: "Numeric Limit Test", Order: 61.1)]
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
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xCommonCommandEse()
        {
            {
                VerdictTest = VerdictTestEnum.Ignore;
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            MyVerdict = Verdict.NotSet;
            UpgradeVerdict(MyVerdict);
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            if (Action == SetAction.Command)
            {
                myInstrument.CommonCommands.SetEse(_enableValueCP);

            }
            else
            {
                _enableValueQR = myInstrument.CommonCommands.GetEse();

                int? result = _enableValueQR;

                if ((result > LowerLimit && result < UpperLimit) && VerdictTest == VerdictTestEnum.InBetween)
                { MyVerdict = Verdict.Pass; }
                else if ((result == ValueEqualTo) && VerdictTest == VerdictTestEnum.EqualTo)
                { MyVerdict = Verdict.Pass; }
                else if ((result >= LowerLimit) && VerdictTest == VerdictTestEnum.GreaterThan)
                { MyVerdict = Verdict.Pass; }
                else if ((result <= UpperLimit) && VerdictTest == VerdictTestEnum.LessThan)
                { MyVerdict = Verdict.Pass; }
                else if (VerdictTest == VerdictTestEnum.Ignore)
                { MyVerdict = Verdict.Pass; }
                else
                { MyVerdict = Verdict.Fail; }
                UpgradeVerdict(MyVerdict);
                if (publishResults)
                {
                    Results.Publish("enableValue", new { Enablevalue = (int)_enableValueQR });
                }
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xCommonCommandEsr
    [Display("*ESR", Groups: new[] { "E364x" }, Description: "Query the Standard Event register.")]
    public class E364xCommonCommandEsr : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_87.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response Properties
        [Output]
        [Display(Group: "Query Response ", Name: "esr", Description: "Returns the current value of the Standard Event register.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public int? _esrQR { get; private set; }
        #endregion
        #region Result Checkbox
        [Display("Publish Results", Group: "Results", Description: "Enable to publish results", Order: 58.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool publishResults { get; set; }
        #endregion
        #region Verdict Properties
        [Output]
        [Display("Verdict", Group: "Verdict", Order: 60.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public Verdict MyVerdict { get; private set; }
        public enum VerdictTestEnum { EqualTo, LessThan, GreaterThan, InBetween, Ignore };
        [Display(Group: "Numeric Limit Test", Name: "Numeric Limit Test", Order: 61.1)]
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
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xCommonCommandEsr()
        {
            {
                VerdictTest = VerdictTestEnum.Ignore;
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            MyVerdict = Verdict.NotSet;
            UpgradeVerdict(MyVerdict);
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            _esrQR = myInstrument.CommonCommands.GetEsr();

            int? result = _esrQR;

            if ((result > LowerLimit && result < UpperLimit) && VerdictTest == VerdictTestEnum.InBetween)
            { MyVerdict = Verdict.Pass; }
            else if ((result == ValueEqualTo) && VerdictTest == VerdictTestEnum.EqualTo)
            { MyVerdict = Verdict.Pass; }
            else if ((result >= LowerLimit) && VerdictTest == VerdictTestEnum.GreaterThan)
            { MyVerdict = Verdict.Pass; }
            else if ((result <= UpperLimit) && VerdictTest == VerdictTestEnum.LessThan)
            { MyVerdict = Verdict.Pass; }
            else if (VerdictTest == VerdictTestEnum.Ignore)
            { MyVerdict = Verdict.Pass; }
            else
            { MyVerdict = Verdict.Fail; }
            UpgradeVerdict(MyVerdict);
            if (publishResults)
            {
                Results.Publish("esr", new { Esr = (int)_esrQR });
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xCommonCommandIdn
    [Display("*IDN", Groups: new[] { "E364x" }, Description: "Read the power supply’s identification string.")]
    public class E364xCommonCommandIdn : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_53.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response Properties
        [Output]
        [Display(Group: "Query Response ", Name: "idn", Description: "The instrument identification string.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public string _idnQR { get; private set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xCommonCommandIdn()
        {
            {
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            _idnQR = myInstrument.CommonCommands.GetIdn();
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xCommonCommandOpc
    [Display("*OPC", Groups: new[] { "E364x" }, Description: "Set the ‘‘Operation Complete’’ bit (bit 0) of the Standard Event register after the command is executed.  Return ‘‘1’’ to the output buffer after the command is executed.")]
    public class E364xCommonCommandOpc : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_88.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response Properties
        [Output]
        [Display(Group: "Query Response ", Name: "opc", Description: "Return ‘‘1’’ to the output buffer after the command is executed.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public int? _opcQR { get; private set; }
        #endregion
        #region Result Checkbox
        [Display("Publish Results", Group: "Results", Description: "Enable to publish results", Order: 58.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool publishResults { get; set; }
        #endregion
        #region Verdict Properties
        [Output]
        [Display("Verdict", Group: "Verdict", Order: 60.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public Verdict MyVerdict { get; private set; }
        public enum VerdictTestEnum { EqualTo, LessThan, GreaterThan, InBetween, Ignore };
        [Display(Group: "Numeric Limit Test", Name: "Numeric Limit Test", Order: 61.1)]
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
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xCommonCommandOpc()
        {
            {
                VerdictTest = VerdictTestEnum.Ignore;
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            MyVerdict = Verdict.NotSet;
            UpgradeVerdict(MyVerdict);
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            if (Action == SetAction.Command)
            {
                myInstrument.CommonCommands.SetOpc();

            }
            else
            {
                _opcQR = myInstrument.CommonCommands.GetOpc();

                int? result = _opcQR;

                if ((result > LowerLimit && result < UpperLimit) && VerdictTest == VerdictTestEnum.InBetween)
                { MyVerdict = Verdict.Pass; }
                else if ((result == ValueEqualTo) && VerdictTest == VerdictTestEnum.EqualTo)
                { MyVerdict = Verdict.Pass; }
                else if ((result >= LowerLimit) && VerdictTest == VerdictTestEnum.GreaterThan)
                { MyVerdict = Verdict.Pass; }
                else if ((result <= UpperLimit) && VerdictTest == VerdictTestEnum.LessThan)
                { MyVerdict = Verdict.Pass; }
                else if (VerdictTest == VerdictTestEnum.Ignore)
                { MyVerdict = Verdict.Pass; }
                else
                { MyVerdict = Verdict.Fail; }
                UpgradeVerdict(MyVerdict);
                if (publishResults)
                {
                    Results.Publish("opc", new { Opc = (int)_opcQR });
                }
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xCommonCommandPsc
    [Display("*PSC", Groups: new[] { "E364x" }, Description: "This command clears the Status Byte and the Standard Event register enable masks when power is turned on (*PSC 1).   Query the power-on status clear setting.")]
    public class E364xCommonCommandPsc : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_90.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response Properties
        [Display(Group: "Command Parameter ", Name: "psc", Description: "the Power-on status clear.", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public int? _pscCP { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "psc", Description: "Returns the current the power-on status clear setting.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public int? _pscQR { get; private set; }
        #endregion
        #region Result Checkbox
        [Display("Publish Results", Group: "Results", Description: "Enable to publish results", Order: 58.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool publishResults { get; set; }
        #endregion
        #region Verdict Properties
        [Output]
        [Display("Verdict", Group: "Verdict", Order: 60.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public Verdict MyVerdict { get; private set; }
        public enum VerdictTestEnum { EqualTo, LessThan, GreaterThan, InBetween, Ignore };
        [Display(Group: "Numeric Limit Test", Name: "Numeric Limit Test", Order: 61.1)]
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
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xCommonCommandPsc()
        {
            {
                VerdictTest = VerdictTestEnum.Ignore;
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            MyVerdict = Verdict.NotSet;
            UpgradeVerdict(MyVerdict);
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            if (Action == SetAction.Command)
            {
                myInstrument.CommonCommands.SetPsc(_pscCP);

            }
            else
            {
                _pscQR = myInstrument.CommonCommands.GetPsc();

                int? result = _pscQR;

                if ((result > LowerLimit && result < UpperLimit) && VerdictTest == VerdictTestEnum.InBetween)
                { MyVerdict = Verdict.Pass; }
                else if ((result == ValueEqualTo) && VerdictTest == VerdictTestEnum.EqualTo)
                { MyVerdict = Verdict.Pass; }
                else if ((result >= LowerLimit) && VerdictTest == VerdictTestEnum.GreaterThan)
                { MyVerdict = Verdict.Pass; }
                else if ((result <= UpperLimit) && VerdictTest == VerdictTestEnum.LessThan)
                { MyVerdict = Verdict.Pass; }
                else if (VerdictTest == VerdictTestEnum.Ignore)
                { MyVerdict = Verdict.Pass; }
                else
                { MyVerdict = Verdict.Fail; }
                UpgradeVerdict(MyVerdict);
                if (publishResults)
                {
                    Results.Publish("psc", new { Psc = (int)_pscQR });
                }
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xCommonCommandRcl
    [Display("*RCL", Groups: new[] { "E364x" }, Description: "Recall the power supply state stored in the specified storage location.")]
    public class E364xCommonCommandRcl : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_57.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response Properties
        [Display(Group: "Command Parameter ", Name: "rcl", Description: "Controls the power supply state stored in the specified storage location.", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public int? _rclCP { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xCommonCommandRcl()
        {
            {
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            myInstrument.CommonCommands.SetRcl(_rclCP);
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xCommonCommandRst
    [Display("*RST", Groups: new[] { "E364x" }, Description: "Reset the power supply to its power-on state.")]
    public class E364xCommonCommandRst : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_55.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response Properties
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xCommonCommandRst()
        {
            {
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            myInstrument.CommonCommands.SetRst();
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xCommonCommandSav
    [Display("*SAV", Groups: new[] { "E364x" }, Description: "Store (Save) the present state of the power supply to the specified location.")]
    public class E364xCommonCommandSav : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_56.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response Properties
        [Display(Group: "Command Parameter ", Name: "sav", Description: "Store (Save) the present state of the power supply to the specified location.", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public int? _savCP { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xCommonCommandSav()
        {
            {
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            myInstrument.CommonCommands.SetSav(_savCP);
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xCommonCommandSre
    [Display("*SRE", Groups: new[] { "E364x" }, Description: "Enable bits in the Status Byte enable register.  Query the Status Byte Enable register.")]
    public class E364xCommonCommandSre : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_92.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response Properties
        [Display(Group: "Command Parameter ", Name: "enableValue", Description: "The Status Byte enable register.", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public int? _enableValueCP { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "enableValue", Description: "Returns the current value of the Status Byte enable register.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public int? _enableValueQR { get; private set; }
        #endregion
        #region Result Checkbox
        [Display("Publish Results", Group: "Results", Description: "Enable to publish results", Order: 58.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool publishResults { get; set; }
        #endregion
        #region Verdict Properties
        [Output]
        [Display("Verdict", Group: "Verdict", Order: 60.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public Verdict MyVerdict { get; private set; }
        public enum VerdictTestEnum { EqualTo, LessThan, GreaterThan, InBetween, Ignore };
        [Display(Group: "Numeric Limit Test", Name: "Numeric Limit Test", Order: 61.1)]
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
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xCommonCommandSre()
        {
            {
                VerdictTest = VerdictTestEnum.Ignore;
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            MyVerdict = Verdict.NotSet;
            UpgradeVerdict(MyVerdict);
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            if (Action == SetAction.Command)
            {
                myInstrument.CommonCommands.SetSre(_enableValueCP);

            }
            else
            {
                _enableValueQR = myInstrument.CommonCommands.GetSre();

                int? result = _enableValueQR;

                if ((result > LowerLimit && result < UpperLimit) && VerdictTest == VerdictTestEnum.InBetween)
                { MyVerdict = Verdict.Pass; }
                else if ((result == ValueEqualTo) && VerdictTest == VerdictTestEnum.EqualTo)
                { MyVerdict = Verdict.Pass; }
                else if ((result >= LowerLimit) && VerdictTest == VerdictTestEnum.GreaterThan)
                { MyVerdict = Verdict.Pass; }
                else if ((result <= UpperLimit) && VerdictTest == VerdictTestEnum.LessThan)
                { MyVerdict = Verdict.Pass; }
                else if (VerdictTest == VerdictTestEnum.Ignore)
                { MyVerdict = Verdict.Pass; }
                else
                { MyVerdict = Verdict.Fail; }
                UpgradeVerdict(MyVerdict);
                if (publishResults)
                {
                    Results.Publish("enableValue", new { Enablevalue = (int)_enableValueQR });
                }
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xCommonCommandStb
    [Display("*STB", Groups: new[] { "E364x" }, Description: "Query the Status Byte summary register.")]
    public class E364xCommonCommandStb : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_94.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response Properties
        [Output]
        [Display(Group: "Query Response ", Name: "stb", Description: "Returns the current value of the Status Byte summary register.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public int? _stbQR { get; private set; }
        #endregion
        #region Result Checkbox
        [Display("Publish Results", Group: "Results", Description: "Enable to publish results", Order: 58.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool publishResults { get; set; }
        #endregion
        #region Verdict Properties
        [Output]
        [Display("Verdict", Group: "Verdict", Order: 60.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public Verdict MyVerdict { get; private set; }
        public enum VerdictTestEnum { EqualTo, LessThan, GreaterThan, InBetween, Ignore };
        [Display(Group: "Numeric Limit Test", Name: "Numeric Limit Test", Order: 61.1)]
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
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xCommonCommandStb()
        {
            {
                VerdictTest = VerdictTestEnum.Ignore;
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            MyVerdict = Verdict.NotSet;
            UpgradeVerdict(MyVerdict);
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            _stbQR = myInstrument.CommonCommands.GetStb();

            int? result = _stbQR;

            if ((result > LowerLimit && result < UpperLimit) && VerdictTest == VerdictTestEnum.InBetween)
            { MyVerdict = Verdict.Pass; }
            else if ((result == ValueEqualTo) && VerdictTest == VerdictTestEnum.EqualTo)
            { MyVerdict = Verdict.Pass; }
            else if ((result >= LowerLimit) && VerdictTest == VerdictTestEnum.GreaterThan)
            { MyVerdict = Verdict.Pass; }
            else if ((result <= UpperLimit) && VerdictTest == VerdictTestEnum.LessThan)
            { MyVerdict = Verdict.Pass; }
            else if (VerdictTest == VerdictTestEnum.Ignore)
            { MyVerdict = Verdict.Pass; }
            else
            { MyVerdict = Verdict.Fail; }
            UpgradeVerdict(MyVerdict);
            if (publishResults)
            {
                Results.Publish("stb", new { Stb = (int)_stbQR });
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xCommonCommandTrg
    [Display("*TRG", Groups: new[] { "E364x" }, Description: "Generate a trigger to the trigger subsystem that has selected a bus (software) trigger as its source (TRIG:SOUR BUS).")]
    public class E364xCommonCommandTrg : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_38.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response Properties
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xCommonCommandTrg()
        {
            {
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            myInstrument.CommonCommands.SetTrg();
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xCommonCommandTst
    [Display("*TST", Groups: new[] { "E364x" }, Description: "Perform a complete self-test of the power supply.")]
    public class E364xCommonCommandTst : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_54.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response Properties
        [Output]
        [Display(Group: "Query Response ", Name: "tst", Description: "Returns the complete self-test of the power supply.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public int? _tstQR { get; private set; }
        #endregion
        #region Result Checkbox
        [Display("Publish Results", Group: "Results", Description: "Enable to publish results", Order: 58.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool publishResults { get; set; }
        #endregion
        #region Verdict Properties
        [Output]
        [Display("Verdict", Group: "Verdict", Order: 60.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public Verdict MyVerdict { get; private set; }
        public enum VerdictTestEnum { EqualTo, LessThan, GreaterThan, InBetween, Ignore };
        [Display(Group: "Numeric Limit Test", Name: "Numeric Limit Test", Order: 61.1)]
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
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xCommonCommandTst()
        {
            {
                VerdictTest = VerdictTestEnum.Ignore;
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            MyVerdict = Verdict.NotSet;
            UpgradeVerdict(MyVerdict);
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            _tstQR = myInstrument.CommonCommands.GetTst();

            int? result = _tstQR;

            if ((result > LowerLimit && result < UpperLimit) && VerdictTest == VerdictTestEnum.InBetween)
            { MyVerdict = Verdict.Pass; }
            else if ((result == ValueEqualTo) && VerdictTest == VerdictTestEnum.EqualTo)
            { MyVerdict = Verdict.Pass; }
            else if ((result >= LowerLimit) && VerdictTest == VerdictTestEnum.GreaterThan)
            { MyVerdict = Verdict.Pass; }
            else if ((result <= UpperLimit) && VerdictTest == VerdictTestEnum.LessThan)
            { MyVerdict = Verdict.Pass; }
            else if (VerdictTest == VerdictTestEnum.Ignore)
            { MyVerdict = Verdict.Pass; }
            else
            { MyVerdict = Verdict.Fail; }
            UpgradeVerdict(MyVerdict);
            if (publishResults)
            {
                Results.Publish("tst", new { Tst = (int)_tstQR });
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xCommonCommandWai
    [Display("*WAI", Groups: new[] { "E364x" }, Description: "Instruct the power supply to wait for all pending operations to complete before executing any additional commands over the interface.")]
    public class E364xCommonCommandWai : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_95.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response Properties
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xCommonCommandWai()
        {
            {
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            myInstrument.CommonCommands.SetWai();
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #endregion
    #region Root Node Classes
    #region APPLy Node Classes
    #region E364xApplyStep
    [Display("APPLy", Groups: new[] { "E364x", }, Description: "This command is combination of VOLTage and CURRent commands.  Query the power supply’s present voltage and current setting values and returns a quoted string.")]
    public class E364xApplyStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_1.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "voltage Custom Input", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public bool _voltageCPCustomInput { get; set; }
        [Display(Group: "Command Parameter ", Name: "voltage", Description: "Voltage.", Order: 30.2)]
        [EnabledIf("_voltageCPCustomInput", false, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public defMinMax _voltageCP { get; set; }
        [Display(Group: "Command Parameter ", Name: "voltage", Description: "Voltage.", Order: 30.3)]
        [EnabledIf("_voltageCPCustomInput", true, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public string _voltageCPStr { get; set; }
        [Display(Group: "Command Parameter ", Name: "current Custom Input", Order: 30.4)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public bool _currentCPCustomInput { get; set; }
        [Display(Group: "Command Parameter ", Name: "current", Description: "Current.", Order: 30.5)]
        [EnabledIf("_currentCPCustomInput", false, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public defMinMax _currentCP { get; set; }
        [Display(Group: "Command Parameter ", Name: "current", Description: "Current.", Order: 30.6)]
        [EnabledIf("_currentCPCustomInput", true, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public string _currentCPStr { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "apply", Description: "Returns the power supply’s present voltage and current setting values and returns a quoted string.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public string _applyQR { get; private set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xApplyStep()
        {
            {
                Name = ":APPLy";
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            if (Action == SetAction.Command)
            {
                myInstrument.Apply.Set((_voltageCPCustomInput ? _voltageCPStr : _voltageCP.ToString()), (_currentCPCustomInput ? _currentCPStr : _currentCP.ToString()));

            }
            else
            {
                _applyQR = myInstrument.Apply.Get();
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #endregion
    #region CALibration Node Classes
    #region E364xCalibrationCountStep
    [Display("COUNt", Groups: new[] { "E364x", "CALibration" }, Description: "Query the power supply to determine the number of times it has been calibrated.")]
    public class E364xCalibrationCountStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_59.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Output]
        [Display(Group: "Query Response ", Name: "count", Description: "Returns the power supply to determine the number of times it has been calibrated.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public int? _countQR { get; private set; }
        #endregion
        #region Result Checkbox
        [Display("Publish Results", Group: "Results", Description: "Enable to publish results", Order: 58.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool publishResults { get; set; }
        #endregion
        #region Verdict Properties
        [Output]
        [Display("Verdict", Group: "Verdict", Order: 60.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public Verdict MyVerdict { get; private set; }
        public enum VerdictTestEnum { EqualTo, LessThan, GreaterThan, InBetween, Ignore };
        [Display(Group: "Numeric Limit Test", Name: "Numeric Limit Test", Order: 61.1)]
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
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xCalibrationCountStep()
        {
            {
                Name = "CALibration:COUNt";
                VerdictTest = VerdictTestEnum.Ignore;
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            MyVerdict = Verdict.NotSet;
            UpgradeVerdict(MyVerdict);
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            _countQR = myInstrument.Calibration.GetCount();

            int? result = _countQR;

            if ((result > LowerLimit && result < UpperLimit) && VerdictTest == VerdictTestEnum.InBetween)
            { MyVerdict = Verdict.Pass; }
            else if ((result == ValueEqualTo) && VerdictTest == VerdictTestEnum.EqualTo)
            { MyVerdict = Verdict.Pass; }
            else if ((result >= LowerLimit) && VerdictTest == VerdictTestEnum.GreaterThan)
            { MyVerdict = Verdict.Pass; }
            else if ((result <= UpperLimit) && VerdictTest == VerdictTestEnum.LessThan)
            { MyVerdict = Verdict.Pass; }
            else if (VerdictTest == VerdictTestEnum.Ignore)
            { MyVerdict = Verdict.Pass; }
            else
            { MyVerdict = Verdict.Fail; }
            UpgradeVerdict(MyVerdict);
            if (publishResults)
            {
                Results.Publish("count", new { Count = (int)_countQR });
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xCalibrationCurrentDataStep
    [Display("[DATA]", Groups: new[] { "E364x", "CALibration", "CURRent" }, Description: "This command can only be used after calibration is unsecured and the output state is ON.")]
    public class E364xCalibrationCurrentDataStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_60.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "numericValue", Description: "The numeric value.", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public double? _numericValueCP { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xCalibrationCurrentDataStep()
        {
            {
                Name = "CALibration:CURRent:[DATA]";
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            myInstrument.Calibration.SetCurrentData(_numericValueCP);
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xCalibrationCurrentLevelStep
    [Display("LEVel", Groups: new[] { "E364x", "CALibration", "CURRent" }, Description: "This command can only be used after calibration is unsecured and the output state is ON.")]
    public class E364xCalibrationCurrentLevelStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_61.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "preset Custom Input", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public bool _presetCPCustomInput { get; set; }
        [Display(Group: "Command Parameter ", Name: "preset", Description: "MINimum | MIDdle | MAXimum", Order: 30.2)]
        [EnabledIf("_presetCPCustomInput", false, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public minMidMax _presetCP { get; set; }
        [Display(Group: "Command Parameter ", Name: "preset", Description: "MINimum | MIDdle | MAXimum", Order: 30.3)]
        [EnabledIf("_presetCPCustomInput", true, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public string _presetCPStr { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xCalibrationCurrentLevelStep()
        {
            {
                Name = "CALibration:CURRent:LEVel";
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            myInstrument.Calibration.SetCurrentLevel((_presetCPCustomInput ? _presetCPStr : _presetCP.ToString()));
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xCalibrationSecureCodeStep
    [Display("CODE", Groups: new[] { "E364x", "CALibration", "SECure" }, Description: "Enter a new security code.")]
    public class E364xCalibrationSecureCodeStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_62.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "code", Description: "The new security code.", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public string _codeCP { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xCalibrationSecureCodeStep()
        {
            {
                Name = "CALibration:SECure:CODE";
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            myInstrument.Calibration.SetSecureCode(_codeCP);
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xCalibrationSecureStateStep
    [Display("STATe", Groups: new[] { "E364x", "CALibration", "SECure" }, Description: "Unsecure or secure the power supply with a security for calibration.  Query the secured state for calibration of the power supply.")]
    public class E364xCalibrationSecureStateStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_63.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "state", Description: "Enable/disable the function.", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public bool _stateCP { get; set; }
        [Display(Group: "Command Parameter ", Name: "quotedCode", Description: "The quoted code.", Order: 30.2)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public string _quotedCodeCP { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "state", Description: "Returns the current value of the function.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public bool _stateQR { get; private set; }
        #endregion
        #region Verdict Properties
        [Output]
        [Display("Verdict", Group: "Verdict", Order: 60.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public Verdict MyVerdict { get; private set; }
        public enum VerdictTestEnum { EqualTo, Ignore };
        [Display(Group: "Boolean Test", Name: "Boolean Test", Order: 61.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public VerdictTestEnum VerdictTest { get; set; }

        [Display(Group: "Boolean Test", Name: "Test Value", Order: 61.2)]
        [EnabledIf("VerdictTest", VerdictTestEnum.EqualTo, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool TestValue { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xCalibrationSecureStateStep()
        {
            {
                Name = "CALibration:SECure:STATe";
                VerdictTest = VerdictTestEnum.Ignore;
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            MyVerdict = Verdict.NotSet;
            UpgradeVerdict(MyVerdict);
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            if (Action == SetAction.Command)
            {
                myInstrument.Calibration.SetSecureState(_stateCP, _quotedCodeCP);

            }
            else
            {
                _stateQR = myInstrument.Calibration.GetSecureState();

                bool result = _stateQR;

                if (result == TestValue && VerdictTest == VerdictTestEnum.EqualTo)
                { MyVerdict = Verdict.Pass; }
                else if (VerdictTest == VerdictTestEnum.Ignore)
                { MyVerdict = Verdict.Pass; }
                else { MyVerdict = Verdict.Fail; }
                UpgradeVerdict(MyVerdict);
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xCalibrationStringStep
    [Display("STRing", Groups: new[] { "E364x", "CALibration" }, Description: "Record calibration information about your power supply.  Query the calibration message and returns a quoted string.")]
    public class E364xCalibrationStringStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_65.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "quotedString", Description: "The calibration information about your power supply.", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public string _quotedStringCP { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "quotedString", Description: "Returns the current calibration information about your power supply.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public string _quotedStringQR { get; private set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xCalibrationStringStep()
        {
            {
                Name = "CALibration:STRing";
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            if (Action == SetAction.Command)
            {
                myInstrument.Calibration.SetString(_quotedStringCP);

            }
            else
            {
                _quotedStringQR = myInstrument.Calibration.GetString();
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xCalibrationVoltageDataStep
    [Display("[DATA]", Groups: new[] { "E364x", "CALibration", "VOLTage" }, Description: "This command can only be used after calibration is unsecured and the output state is ON.")]
    public class E364xCalibrationVoltageDataStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_67.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "numericValue", Description: "The numeric value.", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public double? _numericValueCP { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xCalibrationVoltageDataStep()
        {
            {
                Name = "CALibration:VOLTage:[DATA]";
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            myInstrument.Calibration.SetVoltageData(_numericValueCP);
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xCalibrationVoltageLevelStep
    [Display("LEVel", Groups: new[] { "E364x", "CALibration", "VOLTage" }, Description: "This command can only be used after calibration is unsecured and the output state is ON.")]
    public class E364xCalibrationVoltageLevelStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_68.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "preset Custom Input", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public bool _presetCPCustomInput { get; set; }
        [Display(Group: "Command Parameter ", Name: "preset", Description: "MINimum | MIDdle | MAXimum", Order: 30.2)]
        [EnabledIf("_presetCPCustomInput", false, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public minMidMax _presetCP { get; set; }
        [Display(Group: "Command Parameter ", Name: "preset", Description: "MINimum | MIDdle | MAXimum", Order: 30.3)]
        [EnabledIf("_presetCPCustomInput", true, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public string _presetCPStr { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xCalibrationVoltageLevelStep()
        {
            {
                Name = "CALibration:VOLTage:LEVel";
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            myInstrument.Calibration.SetVoltageLevel((_presetCPCustomInput ? _presetCPStr : _presetCP.ToString()));
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xCalibrationVoltageProtectionStep
    [Display("PROTection", Groups: new[] { "E364x", "CALibration", "VOLTage" }, Description: "Calibrate the overvoltage protection circuit of the power supply.")]
    public class E364xCalibrationVoltageProtectionStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_69.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xCalibrationVoltageProtectionStep()
        {
            {
                Name = "CALibration:VOLTage:PROTection";
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            myInstrument.Calibration.SetVoltageProtection();
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #endregion
    #region DISPlay Node Classes
    #region E364xDisplayWindowModeStep
    [Display("MODE", Groups: new[] { "E364x", "DISPlay", "[WINDow]" }, Description: "Set the front-panel display mode of the power supply.  Query the state of the display mode.")]
    public class E364xDisplayWindowModeStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_40.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "mode Custom Input", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public bool _modeCPCustomInput { get; set; }
        [Display(Group: "Command Parameter ", Name: "mode", Description: "The front-panel display mode of the power supply.", Order: 30.2)]
        [EnabledIf("_modeCPCustomInput", false, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public mode _modeCP { get; set; }
        [Display(Group: "Command Parameter ", Name: "mode", Description: "The front-panel display mode of the power supply.", Order: 30.3)]
        [EnabledIf("_modeCPCustomInput", true, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public string _modeCPStr { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "mode", Description: "Returns the current front-panel display mode of the power supply.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public mode _modeQR { get; private set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xDisplayWindowModeStep()
        {
            {
                Name = "DISPlay:[WINDow]:MODE";
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            if (Action == SetAction.Command)
            {
                myInstrument.Display.SetWindowMode((_modeCPCustomInput ? _modeCPStr : _modeCP.ToString()));

            }
            else
            {
                _modeQR = myInstrument.Display.GetWindowMode();
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xDisplayWindowStateStep
    [Display("[STATe]", Groups: new[] { "E364x", "DISPlay", "[WINDow]" }, Description: "Turn the front-panel display off or on.   Query the front-panel display setting.")]
    public class E364xDisplayWindowStateStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_39.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "state", Description: "Enable/disable the front-panel display.", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public bool _stateCP { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "state", Description: "Returns the current value of the front-panel display.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public bool _stateQR { get; private set; }
        #endregion
        #region Verdict Properties
        [Output]
        [Display("Verdict", Group: "Verdict", Order: 60.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public Verdict MyVerdict { get; private set; }
        public enum VerdictTestEnum { EqualTo, Ignore };
        [Display(Group: "Boolean Test", Name: "Boolean Test", Order: 61.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public VerdictTestEnum VerdictTest { get; set; }

        [Display(Group: "Boolean Test", Name: "Test Value", Order: 61.2)]
        [EnabledIf("VerdictTest", VerdictTestEnum.EqualTo, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool TestValue { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xDisplayWindowStateStep()
        {
            {
                Name = "DISPlay:[WINDow]:[STATe]";
                VerdictTest = VerdictTestEnum.Ignore;
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            MyVerdict = Verdict.NotSet;
            UpgradeVerdict(MyVerdict);
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            if (Action == SetAction.Command)
            {
                myInstrument.Display.SetWindowState(_stateCP);

            }
            else
            {
                _stateQR = myInstrument.Display.GetWindowState();

                bool result = _stateQR;

                if (result == TestValue && VerdictTest == VerdictTestEnum.EqualTo)
                { MyVerdict = Verdict.Pass; }
                else if (VerdictTest == VerdictTestEnum.Ignore)
                { MyVerdict = Verdict.Pass; }
                else { MyVerdict = Verdict.Fail; }
                UpgradeVerdict(MyVerdict);
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xDisplayWindowTextClearStep
    [Display("CLEar", Groups: new[] { "E364x", "DISPlay", "[WINDow]", "TEXT" }, Description: "Clear the message displayed on the front panel.")]
    public class E364xDisplayWindowTextClearStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_45.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xDisplayWindowTextClearStep()
        {
            {
                Name = "DISPlay:[WINDow]:TEXT:CLEar";
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            myInstrument.Display.SetWindowTextClear();
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xDisplayWindowTextDataStep
    [Display("[DATA]", Groups: new[] { "E364x", "DISPlay", "[WINDow]", "TEXT" }, Description: "Display a message on the front panel.   Query the message sent to the front panel and returns a quoted string.")]
    public class E364xDisplayWindowTextDataStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_43.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "quotedString", Description: "The front panel message.", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public string _quotedStringCP { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "quotedString", Description: "Returns the front panel message.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public string _quotedStringQR { get; private set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xDisplayWindowTextDataStep()
        {
            {
                Name = "DISPlay:[WINDow]:TEXT:[DATA]";
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            if (Action == SetAction.Command)
            {
                myInstrument.Display.SetWindowTextData(_quotedStringCP);

            }
            else
            {
                _quotedStringQR = myInstrument.Display.GetWindowTextData();
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #endregion
    #region INITiate Node Classes
    #region E364xInitiateImmediateStep
    [Display("[IMMediate]", Groups: new[] { "E364x", "INITiate" }, Description: "Cause the trigger system to initiate.")]
    public class E364xInitiateImmediateStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_33.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xInitiateImmediateStep()
        {
            {
                Name = "INITiate:[IMMediate]";
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            myInstrument.Initiate.SetImmediate();
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #endregion
    #region INSTrument Node Classes
    #region E364xInstrumentSelectStep
    [Display("[SELect]", Groups: new[] { "E364x", "INSTrument" }, Description: "Select the output to be programmed one of the two outputs by the output identifier.  Return the currently selected output by the INSTrument{:SELect] or INSTrument:NSELect command.")]
    public class E364xInstrumentSelectStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_9.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "select Custom Input", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public bool _selectCPCustomInput { get; set; }
        [Display(Group: "Command Parameter ", Name: "select", Description: "The output identifier.", Order: 30.2)]
        [EnabledIf("_selectCPCustomInput", false, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public channel _selectCP { get; set; }
        [Display(Group: "Command Parameter ", Name: "select", Description: "The output identifier.", Order: 30.3)]
        [EnabledIf("_selectCPCustomInput", true, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public string _selectCPStr { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "select", Description: "Returns the current output identifier.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public channel _selectQR { get; private set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xInstrumentSelectStep()
        {
            {
                Name = "INSTrument:[SELect]";
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            if (Action == SetAction.Command)
            {
                myInstrument.Instrument.SetSelect((_selectCPCustomInput ? _selectCPStr : _selectCP.ToString()));

            }
            else
            {
                _selectQR = myInstrument.Instrument.GetSelect();
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xInstrumentNselectStep
    [Display("NSELect", Groups: new[] { "E364x", "INSTrument" }, Description: "Select the output to be programmed one of the two outputs by a numeric value instead of the output identifier used in the INSTrument:NSELect or INSTrument[:SELect] command.  Return the currently selected output by the INSTrument[SELect]or INSTrument[SELect] command.")]
    public class E364xInstrumentNselectStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_11.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "nselect", Description: "Instrument output.", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public int? _nselectCP { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "nselect", Description: "Return the currently selected output by the INSTrument[SELect]", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public int? _nselectQR { get; private set; }
        #endregion
        #region Result Checkbox
        [Display("Publish Results", Group: "Results", Description: "Enable to publish results", Order: 58.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool publishResults { get; set; }
        #endregion
        #region Verdict Properties
        [Output]
        [Display("Verdict", Group: "Verdict", Order: 60.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public Verdict MyVerdict { get; private set; }
        public enum VerdictTestEnum { EqualTo, LessThan, GreaterThan, InBetween, Ignore };
        [Display(Group: "Numeric Limit Test", Name: "Numeric Limit Test", Order: 61.1)]
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
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xInstrumentNselectStep()
        {
            {
                Name = "INSTrument:NSELect";
                VerdictTest = VerdictTestEnum.Ignore;
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            MyVerdict = Verdict.NotSet;
            UpgradeVerdict(MyVerdict);
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            if (Action == SetAction.Command)
            {
                myInstrument.Instrument.SetNselect(_nselectCP);

            }
            else
            {
                _nselectQR = myInstrument.Instrument.GetNselect();

                int? result = _nselectQR;

                if ((result > LowerLimit && result < UpperLimit) && VerdictTest == VerdictTestEnum.InBetween)
                { MyVerdict = Verdict.Pass; }
                else if ((result == ValueEqualTo) && VerdictTest == VerdictTestEnum.EqualTo)
                { MyVerdict = Verdict.Pass; }
                else if ((result >= LowerLimit) && VerdictTest == VerdictTestEnum.GreaterThan)
                { MyVerdict = Verdict.Pass; }
                else if ((result <= UpperLimit) && VerdictTest == VerdictTestEnum.LessThan)
                { MyVerdict = Verdict.Pass; }
                else if (VerdictTest == VerdictTestEnum.Ignore)
                { MyVerdict = Verdict.Pass; }
                else
                { MyVerdict = Verdict.Fail; }
                UpgradeVerdict(MyVerdict);
                if (publishResults)
                {
                    Results.Publish("nselect", new { Nselect = (int)_nselectQR });
                }
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xInstrumentCoupleTriggerStep
    [Display("[TRIGger]", Groups: new[] { "E364x", "INSTrument", "COUPle" }, Description: "Enable or disable a coupling between two logical outputs of the power supply.  Query the output coupling state of the power supply.")]
    public class E364xInstrumentCoupleTriggerStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_13.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "state", Description: "Enable/disable the power supply function.", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public bool _stateCP { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "state", Description: "Returns the current value of the power supply function.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public int? _stateQR { get; private set; }
        #endregion
        #region Result Checkbox
        [Display("Publish Results", Group: "Results", Description: "Enable to publish results", Order: 58.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool publishResults { get; set; }
        #endregion
        #region Verdict Properties
        [Output]
        [Display("Verdict", Group: "Verdict", Order: 60.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public Verdict MyVerdict { get; private set; }
        public enum VerdictTestEnum { EqualTo, LessThan, GreaterThan, InBetween, Ignore };
        [Display(Group: "Numeric Limit Test", Name: "Numeric Limit Test", Order: 61.1)]
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
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xInstrumentCoupleTriggerStep()
        {
            {
                Name = "INSTrument:COUPle:[TRIGger]";
                VerdictTest = VerdictTestEnum.Ignore;
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            MyVerdict = Verdict.NotSet;
            UpgradeVerdict(MyVerdict);
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            if (Action == SetAction.Command)
            {
                myInstrument.Instrument.SetCoupleTrigger(_stateCP);

            }
            else
            {
                _stateQR = myInstrument.Instrument.GetCoupleTrigger();

                int? result = _stateQR;

                if ((result > LowerLimit && result < UpperLimit) && VerdictTest == VerdictTestEnum.InBetween)
                { MyVerdict = Verdict.Pass; }
                else if ((result == ValueEqualTo) && VerdictTest == VerdictTestEnum.EqualTo)
                { MyVerdict = Verdict.Pass; }
                else if ((result >= LowerLimit) && VerdictTest == VerdictTestEnum.GreaterThan)
                { MyVerdict = Verdict.Pass; }
                else if ((result <= UpperLimit) && VerdictTest == VerdictTestEnum.LessThan)
                { MyVerdict = Verdict.Pass; }
                else if (VerdictTest == VerdictTestEnum.Ignore)
                { MyVerdict = Verdict.Pass; }
                else
                { MyVerdict = Verdict.Fail; }
                UpgradeVerdict(MyVerdict);
                if (publishResults)
                {
                    Results.Publish("state", new { State = (int)_stateQR });
                }
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #endregion
    #region MEASure Node Classes
    #region E364xMeasureScalarCurrentDcStep
    [Display("[DC]", Groups: new[] { "E364x", "MEASure", "[SCALar]", "CURRent" }, Description: "Query the current measured across the current sense resistor inside the power supply.")]
    public class E364xMeasureScalarCurrentDcStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_15.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Output]
        [Display(Group: "Query Response ", Name: "current", Description: "Returns the current measured across the current sense resistor inside the power supply.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public double? _currentQR { get; private set; }
        #endregion
        #region Result Checkbox
        [Display("Publish Results", Group: "Results", Description: "Enable to publish results", Order: 58.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool publishResults { get; set; }
        #endregion
        #region Verdict Properties
        [Output]
        [Display("Verdict", Group: "Verdict", Order: 60.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public Verdict MyVerdict { get; private set; }
        public enum VerdictTestEnum { EqualTo, LessThan, GreaterThan, InBetween, Ignore };
        [Display(Group: "Numeric Limit Test", Name: "Numeric Limit Test", Order: 61.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public VerdictTestEnum VerdictTest { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Value", Order: 61.2, Description: "The value allowed. If not equal, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.EqualTo, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public double ValueEqualTo { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Lower Limit Value", Order: 61.3, Description: "The minimum value allowed. If less, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.GreaterThan, VerdictTestEnum.InBetween, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public double LowerLimit { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Upper Limit Value", Order: 61.4, Description: "The maximum value allowed. If exceeded, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.LessThan, VerdictTestEnum.InBetween, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public double UpperLimit { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xMeasureScalarCurrentDcStep()
        {
            {
                Name = "MEASure:[SCALar]:CURRent:[DC]";
                VerdictTest = VerdictTestEnum.Ignore;
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            MyVerdict = Verdict.NotSet;
            UpgradeVerdict(MyVerdict);
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            _currentQR = myInstrument.Measure.GetScalarCurrentDc();

            double? result = _currentQR;

            if ((result > LowerLimit && result < UpperLimit) && VerdictTest == VerdictTestEnum.InBetween)
            { MyVerdict = Verdict.Pass; }
            else if ((result == ValueEqualTo) && VerdictTest == VerdictTestEnum.EqualTo)
            { MyVerdict = Verdict.Pass; }
            else if ((result >= LowerLimit) && VerdictTest == VerdictTestEnum.GreaterThan)
            { MyVerdict = Verdict.Pass; }
            else if ((result <= UpperLimit) && VerdictTest == VerdictTestEnum.LessThan)
            { MyVerdict = Verdict.Pass; }
            else if (VerdictTest == VerdictTestEnum.Ignore)
            { MyVerdict = Verdict.Pass; }
            else
            { MyVerdict = Verdict.Fail; }
            UpgradeVerdict(MyVerdict);
            if (publishResults)
            {
                Results.Publish("current", new { Current = (double)_currentQR });
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xMeasureScalarVoltageDcStep
    [Display("[DC]", Groups: new[] { "E364x", "MEASure", "[SCALar]", "[VOLTage]" }, Description: "Query the voltage measured at the sense terminals of the power supply.")]
    public class E364xMeasureScalarVoltageDcStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_16.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Output]
        [Display(Group: "Query Response ", Name: "voltage", Description: "Returns the voltage measured at the sense terminals of the power supply.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public double? _voltageQR { get; private set; }
        #endregion
        #region Result Checkbox
        [Display("Publish Results", Group: "Results", Description: "Enable to publish results", Order: 58.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool publishResults { get; set; }
        #endregion
        #region Verdict Properties
        [Output]
        [Display("Verdict", Group: "Verdict", Order: 60.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public Verdict MyVerdict { get; private set; }
        public enum VerdictTestEnum { EqualTo, LessThan, GreaterThan, InBetween, Ignore };
        [Display(Group: "Numeric Limit Test", Name: "Numeric Limit Test", Order: 61.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public VerdictTestEnum VerdictTest { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Value", Order: 61.2, Description: "The value allowed. If not equal, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.EqualTo, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public double ValueEqualTo { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Lower Limit Value", Order: 61.3, Description: "The minimum value allowed. If less, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.GreaterThan, VerdictTestEnum.InBetween, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public double LowerLimit { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Upper Limit Value", Order: 61.4, Description: "The maximum value allowed. If exceeded, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.LessThan, VerdictTestEnum.InBetween, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public double UpperLimit { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xMeasureScalarVoltageDcStep()
        {
            {
                Name = "MEASure:[SCALar]:[VOLTage]:[DC]";
                VerdictTest = VerdictTestEnum.Ignore;
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            MyVerdict = Verdict.NotSet;
            UpgradeVerdict(MyVerdict);
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            _voltageQR = myInstrument.Measure.GetScalarVoltageDc();

            double? result = _voltageQR;

            if ((result > LowerLimit && result < UpperLimit) && VerdictTest == VerdictTestEnum.InBetween)
            { MyVerdict = Verdict.Pass; }
            else if ((result == ValueEqualTo) && VerdictTest == VerdictTestEnum.EqualTo)
            { MyVerdict = Verdict.Pass; }
            else if ((result >= LowerLimit) && VerdictTest == VerdictTestEnum.GreaterThan)
            { MyVerdict = Verdict.Pass; }
            else if ((result <= UpperLimit) && VerdictTest == VerdictTestEnum.LessThan)
            { MyVerdict = Verdict.Pass; }
            else if (VerdictTest == VerdictTestEnum.Ignore)
            { MyVerdict = Verdict.Pass; }
            else
            { MyVerdict = Verdict.Fail; }
            UpgradeVerdict(MyVerdict);
            if (publishResults)
            {
                Results.Publish("voltage", new { Voltage = (double)_voltageQR });
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #endregion
    #region MEMory Node Classes
    #region E364xMemoryStateNameStep
    [Display("NAME", Groups: new[] { "E364x", "MEMory", "STATe" }, Description: "Assign a name to the specified storage location.")]
    public class E364xMemoryStateNameStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_58.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "name", Description: "The storage location.", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public int? _nameCP { get; set; }
        [Display(Group: "Command Parameter ", Name: "quotedName", Description: "Name of the storage location.", Order: 30.2)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public string _quotedNameCP { get; set; }
        [Display(Group: "Query Parameter ", Name: "name", Description: "The storage location.", Order: 40.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public int? _nameQP { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "quotedName", Description: "Returns the current value of name of the storage location.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public string _quotedNameQR { get; private set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xMemoryStateNameStep()
        {
            {
                Name = "MEMory:STATe:NAME";
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            if (Action == SetAction.Command)
            {
                myInstrument.Memory.SetStateName(_nameCP, _quotedNameCP);

            }
            else
            {
                _quotedNameQR = myInstrument.Memory.GetStateName(_nameQP);
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #endregion
    #region OUTPut Node Classes
    #region E364xOutputStateStep
    [Display("[STATe]", Groups: new[] { "E364x", "OUTPut" }, Description: "Enable or disable the outputs of the power supply.   Query the output state of the power supply.")]
    public class E364xOutputStateStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_46.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "state", Description: "Enable/disable the outputs of the power supply.", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public bool _stateCP { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "state", Description: "Returns the current value of the outputs of the power supply.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public bool _stateQR { get; private set; }
        #endregion
        #region Verdict Properties
        [Output]
        [Display("Verdict", Group: "Verdict", Order: 60.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public Verdict MyVerdict { get; private set; }
        public enum VerdictTestEnum { EqualTo, Ignore };
        [Display(Group: "Boolean Test", Name: "Boolean Test", Order: 61.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public VerdictTestEnum VerdictTest { get; set; }

        [Display(Group: "Boolean Test", Name: "Test Value", Order: 61.2)]
        [EnabledIf("VerdictTest", VerdictTestEnum.EqualTo, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool TestValue { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xOutputStateStep()
        {
            {
                Name = "OUTPut:[STATe]";
                VerdictTest = VerdictTestEnum.Ignore;
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            MyVerdict = Verdict.NotSet;
            UpgradeVerdict(MyVerdict);
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            if (Action == SetAction.Command)
            {
                myInstrument.Output.SetState(_stateCP);

            }
            else
            {
                _stateQR = myInstrument.Output.GetState();

                bool result = _stateQR;

                if (result == TestValue && VerdictTest == VerdictTestEnum.EqualTo)
                { MyVerdict = Verdict.Pass; }
                else if (VerdictTest == VerdictTestEnum.Ignore)
                { MyVerdict = Verdict.Pass; }
                else { MyVerdict = Verdict.Fail; }
                UpgradeVerdict(MyVerdict);
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xOutputRelayStateStep
    [Display("[STATe]", Groups: new[] { "E364x", "OUTPut", "RELay" }, Description: "Set the state of two TTL signals on the RS-232 connector pin 1 and pin 9. These signals are intended for use with an external relay and relay driver. At *RST, the OUTPUT:RELay state is OFF.    Query the state of the TTL relay logic signals.")]
    public class E364xOutputRelayStateStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_48.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "state", Description: "State of the function.", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public bool _stateCP { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "state", Description: "Returns the current value of state of the function.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public bool _stateQR { get; private set; }
        #endregion
        #region Verdict Properties
        [Output]
        [Display("Verdict", Group: "Verdict", Order: 60.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public Verdict MyVerdict { get; private set; }
        public enum VerdictTestEnum { EqualTo, Ignore };
        [Display(Group: "Boolean Test", Name: "Boolean Test", Order: 61.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public VerdictTestEnum VerdictTest { get; set; }

        [Display(Group: "Boolean Test", Name: "Test Value", Order: 61.2)]
        [EnabledIf("VerdictTest", VerdictTestEnum.EqualTo, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool TestValue { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xOutputRelayStateStep()
        {
            {
                Name = "OUTPut:RELay:[STATe]";
                VerdictTest = VerdictTestEnum.Ignore;
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            MyVerdict = Verdict.NotSet;
            UpgradeVerdict(MyVerdict);
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            if (Action == SetAction.Command)
            {
                myInstrument.Output.SetRelayState(_stateCP);

            }
            else
            {
                _stateQR = myInstrument.Output.GetRelayState();

                bool result = _stateQR;

                if (result == TestValue && VerdictTest == VerdictTestEnum.EqualTo)
                { MyVerdict = Verdict.Pass; }
                else if (VerdictTest == VerdictTestEnum.Ignore)
                { MyVerdict = Verdict.Pass; }
                else { MyVerdict = Verdict.Fail; }
                UpgradeVerdict(MyVerdict);
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xOutputTrackStateStep
    [Display("[STATe]", Groups: new[] { "E364x", "OUTPut", "TRACk" }, Description: "Enable or disable the power supply to operate in the track mode.   Query the tracking mode state of the power supply.")]
    public class E364xOutputTrackStateStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_17.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "state", Description: "Enable/disable the power supply to operate in the track mode.", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public bool _stateCP { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "state", Description: "Returns the current value of the power supply to operate in the track mode.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public bool _stateQR { get; private set; }
        #endregion
        #region Verdict Properties
        [Output]
        [Display("Verdict", Group: "Verdict", Order: 60.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public Verdict MyVerdict { get; private set; }
        public enum VerdictTestEnum { EqualTo, Ignore };
        [Display(Group: "Boolean Test", Name: "Boolean Test", Order: 61.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public VerdictTestEnum VerdictTest { get; set; }

        [Display(Group: "Boolean Test", Name: "Test Value", Order: 61.2)]
        [EnabledIf("VerdictTest", VerdictTestEnum.EqualTo, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool TestValue { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xOutputTrackStateStep()
        {
            {
                Name = "OUTPut:TRACk:[STATe]";
                VerdictTest = VerdictTestEnum.Ignore;
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            MyVerdict = Verdict.NotSet;
            UpgradeVerdict(MyVerdict);
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            if (Action == SetAction.Command)
            {
                myInstrument.Output.SetTrackState(_stateCP);

            }
            else
            {
                _stateQR = myInstrument.Output.GetTrackState();

                bool result = _stateQR;

                if (result == TestValue && VerdictTest == VerdictTestEnum.EqualTo)
                { MyVerdict = Verdict.Pass; }
                else if (VerdictTest == VerdictTestEnum.Ignore)
                { MyVerdict = Verdict.Pass; }
                else { MyVerdict = Verdict.Fail; }
                UpgradeVerdict(MyVerdict);
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #endregion
    #region SOURce Node Classes
    #region E364xSourceCurrentLevelImmediateAmplitudeStep
    [Display("[AMPLitude]", Groups: new[] { "E364x", "[SOURce]", "CURRent", "[LEVel]", "[IMMediate]" }, Description: "Program the immediate current level of the power supply.   Return the presently programmed current level of the power supply.")]
    public class E364xSourceCurrentLevelImmediateAmplitudeStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_3.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "current Custom Input", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public bool _currentCPCustomInput { get; set; }
        [Display(Group: "Command Parameter ", Name: "current", Description: "The immediate current level of the power supply.", Order: 30.2)]
        [EnabledIf("_currentCPCustomInput", false, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public minMaxUpDown _currentCP { get; set; }
        [Display(Group: "Command Parameter ", Name: "current", Description: "The immediate current level of the power supply.", Order: 30.3)]
        [EnabledIf("_currentCPCustomInput", true, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public string _currentCPStr { get; set; }
        [Display(Group: "Query Parameter ", Name: "preset Custom Input", Order: 40.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public bool _presetQPCustomInput { get; set; }
        [Display(Group: "Query Parameter ", Name: "preset", Description: "MINimum | MAXimum | UP | DOWN", Order: 40.2)]
        [EnabledIf("_presetQPCustomInput", false, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public minMax _presetQP { get; set; }
        [Display(Group: "Query Parameter ", Name: "preset", Description: "MINimum | MAXimum | UP | DOWN", Order: 40.3)]
        [EnabledIf("_presetQPCustomInput", true, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public string _presetQPStr { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "current", Description: "Returns the current value of the immediate current level of the power supply.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public double? _currentQR { get; private set; }
        #endregion
        #region Result Checkbox
        [Display("Publish Results", Group: "Results", Description: "Enable to publish results", Order: 58.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool publishResults { get; set; }
        #endregion
        #region Verdict Properties
        [Output]
        [Display("Verdict", Group: "Verdict", Order: 60.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public Verdict MyVerdict { get; private set; }
        public enum VerdictTestEnum { EqualTo, LessThan, GreaterThan, InBetween, Ignore };
        [Display(Group: "Numeric Limit Test", Name: "Numeric Limit Test", Order: 61.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public VerdictTestEnum VerdictTest { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Value", Order: 61.2, Description: "The value allowed. If not equal, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.EqualTo, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public double ValueEqualTo { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Lower Limit Value", Order: 61.3, Description: "The minimum value allowed. If less, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.GreaterThan, VerdictTestEnum.InBetween, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public double LowerLimit { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Upper Limit Value", Order: 61.4, Description: "The maximum value allowed. If exceeded, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.LessThan, VerdictTestEnum.InBetween, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public double UpperLimit { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xSourceCurrentLevelImmediateAmplitudeStep()
        {
            {
                Name = "[SOURce]:CURRent:[LEVel]:[IMMediate]:[AMPLitude]";
                VerdictTest = VerdictTestEnum.Ignore;
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            MyVerdict = Verdict.NotSet;
            UpgradeVerdict(MyVerdict);
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            if (Action == SetAction.Command)
            {
                myInstrument.Source.SetCurrentLevelImmediateAmplitude((_currentCPCustomInput ? _currentCPStr : _currentCP.ToString()));

            }
            else
            {
                _currentQR = myInstrument.Source.GetCurrentLevelImmediateAmplitude((_presetQPCustomInput ? _presetQPStr : _presetQP.ToString()));

                double? result = _currentQR;

                if ((result > LowerLimit && result < UpperLimit) && VerdictTest == VerdictTestEnum.InBetween)
                { MyVerdict = Verdict.Pass; }
                else if ((result == ValueEqualTo) && VerdictTest == VerdictTestEnum.EqualTo)
                { MyVerdict = Verdict.Pass; }
                else if ((result >= LowerLimit) && VerdictTest == VerdictTestEnum.GreaterThan)
                { MyVerdict = Verdict.Pass; }
                else if ((result <= UpperLimit) && VerdictTest == VerdictTestEnum.LessThan)
                { MyVerdict = Verdict.Pass; }
                else if (VerdictTest == VerdictTestEnum.Ignore)
                { MyVerdict = Verdict.Pass; }
                else
                { MyVerdict = Verdict.Fail; }
                UpgradeVerdict(MyVerdict);
                if (publishResults)
                {
                    Results.Publish("current", new { Current = (double)_currentQR });
                }
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xSourceCurrentLevelImmediateStepIncrementStep
    [Display("[INCRement]", Groups: new[] { "E364x", "[SOURce]", "CURRent", "[LEVel]", "[IMMediate]", "STEP" }, Description: "Set the step size for current programming with the CURRent UPand CURRentDOWN commands.   Return the value of the step size currently specified.")]
    public class E364xSourceCurrentLevelImmediateStepIncrementStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_5.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "numericValue Custom Input", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public bool _numericValueCPCustomInput { get; set; }
        [Display(Group: "Command Parameter ", Name: "numericValue", Description: "The step size for current programming with the CURRent UPand CURRentDOWN commands.", Order: 30.2)]
        [EnabledIf("_numericValueCPCustomInput", false, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public _default _numericValueCP { get; set; }
        [Display(Group: "Command Parameter ", Name: "numericValue", Description: "The step size for current programming with the CURRent UPand CURRentDOWN commands.", Order: 30.3)]
        [EnabledIf("_numericValueCPCustomInput", true, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public string _numericValueCPStr { get; set; }
        [Display(Group: "Query Parameter ", Name: "default Custom Input", Order: 40.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public bool _defaultQPCustomInput { get; set; }
        [Display(Group: "Query Parameter ", Name: "default", Description: "Default.", Order: 40.2)]
        [EnabledIf("_defaultQPCustomInput", false, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public _default _defaultQP { get; set; }
        [Display(Group: "Query Parameter ", Name: "default", Description: "Default.", Order: 40.3)]
        [EnabledIf("_defaultQPCustomInput", true, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public string _defaultQPStr { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "numericValue", Description: "Return the value of the step size currently specified.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public double? _numericValueQR { get; private set; }
        #endregion
        #region Result Checkbox
        [Display("Publish Results", Group: "Results", Description: "Enable to publish results", Order: 58.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool publishResults { get; set; }
        #endregion
        #region Verdict Properties
        [Output]
        [Display("Verdict", Group: "Verdict", Order: 60.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public Verdict MyVerdict { get; private set; }
        public enum VerdictTestEnum { EqualTo, LessThan, GreaterThan, InBetween, Ignore };
        [Display(Group: "Numeric Limit Test", Name: "Numeric Limit Test", Order: 61.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public VerdictTestEnum VerdictTest { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Value", Order: 61.2, Description: "The value allowed. If not equal, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.EqualTo, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public double ValueEqualTo { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Lower Limit Value", Order: 61.3, Description: "The minimum value allowed. If less, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.GreaterThan, VerdictTestEnum.InBetween, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public double LowerLimit { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Upper Limit Value", Order: 61.4, Description: "The maximum value allowed. If exceeded, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.LessThan, VerdictTestEnum.InBetween, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public double UpperLimit { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xSourceCurrentLevelImmediateStepIncrementStep()
        {
            {
                Name = "[SOURce]:CURRent:[LEVel]:[IMMediate]:STEP:[INCRement]";
                VerdictTest = VerdictTestEnum.Ignore;
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            MyVerdict = Verdict.NotSet;
            UpgradeVerdict(MyVerdict);
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            if (Action == SetAction.Command)
            {
                myInstrument.Source.SetCurrentLevelImmediateStepIncrement((_numericValueCPCustomInput ? _numericValueCPStr : _numericValueCP.ToString()));

            }
            else
            {
                _numericValueQR = myInstrument.Source.GetCurrentLevelImmediateStepIncrement((_defaultQPCustomInput ? _defaultQPStr : _defaultQP.ToString()));

                double? result = _numericValueQR;

                if ((result > LowerLimit && result < UpperLimit) && VerdictTest == VerdictTestEnum.InBetween)
                { MyVerdict = Verdict.Pass; }
                else if ((result == ValueEqualTo) && VerdictTest == VerdictTestEnum.EqualTo)
                { MyVerdict = Verdict.Pass; }
                else if ((result >= LowerLimit) && VerdictTest == VerdictTestEnum.GreaterThan)
                { MyVerdict = Verdict.Pass; }
                else if ((result <= UpperLimit) && VerdictTest == VerdictTestEnum.LessThan)
                { MyVerdict = Verdict.Pass; }
                else if (VerdictTest == VerdictTestEnum.Ignore)
                { MyVerdict = Verdict.Pass; }
                else
                { MyVerdict = Verdict.Fail; }
                UpgradeVerdict(MyVerdict);
                if (publishResults)
                {
                    Results.Publish("numericValue", new { Numericvalue = (double)_numericValueQR });
                }
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xSourceCurrentLevelTriggeredAmplitudeStep
    [Display("[AMPLitude]", Groups: new[] { "E364x", "[SOURce]", "CURRent", "[LEVel]", "TRIGgered" }, Description: "Program the pending triggered current level.   Query the triggered current level presently programmed.")]
    public class E364xSourceCurrentLevelTriggeredAmplitudeStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_7.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "current Custom Input", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public bool _currentCPCustomInput { get; set; }
        [Display(Group: "Command Parameter ", Name: "current", Description: "The the pending triggered current level.", Order: 30.2)]
        [EnabledIf("_currentCPCustomInput", false, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public minMax _currentCP { get; set; }
        [Display(Group: "Command Parameter ", Name: "current", Description: "The the pending triggered current level.", Order: 30.3)]
        [EnabledIf("_currentCPCustomInput", true, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public string _currentCPStr { get; set; }
        [Display(Group: "Query Parameter ", Name: "preset Custom Input", Order: 40.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public bool _presetQPCustomInput { get; set; }
        [Display(Group: "Query Parameter ", Name: "preset", Description: "MINimum | MAXimum", Order: 40.2)]
        [EnabledIf("_presetQPCustomInput", false, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public minMax _presetQP { get; set; }
        [Display(Group: "Query Parameter ", Name: "preset", Description: "MINimum | MAXimum", Order: 40.3)]
        [EnabledIf("_presetQPCustomInput", true, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public string _presetQPStr { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "current", Description: "Returns the triggered current level presently programmed.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public double? _currentQR { get; private set; }
        #endregion
        #region Result Checkbox
        [Display("Publish Results", Group: "Results", Description: "Enable to publish results", Order: 58.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool publishResults { get; set; }
        #endregion
        #region Verdict Properties
        [Output]
        [Display("Verdict", Group: "Verdict", Order: 60.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public Verdict MyVerdict { get; private set; }
        public enum VerdictTestEnum { EqualTo, LessThan, GreaterThan, InBetween, Ignore };
        [Display(Group: "Numeric Limit Test", Name: "Numeric Limit Test", Order: 61.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public VerdictTestEnum VerdictTest { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Value", Order: 61.2, Description: "The value allowed. If not equal, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.EqualTo, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public double ValueEqualTo { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Lower Limit Value", Order: 61.3, Description: "The minimum value allowed. If less, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.GreaterThan, VerdictTestEnum.InBetween, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public double LowerLimit { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Upper Limit Value", Order: 61.4, Description: "The maximum value allowed. If exceeded, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.LessThan, VerdictTestEnum.InBetween, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public double UpperLimit { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xSourceCurrentLevelTriggeredAmplitudeStep()
        {
            {
                Name = "[SOURce]:CURRent:[LEVel]:TRIGgered:[AMPLitude]";
                VerdictTest = VerdictTestEnum.Ignore;
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            MyVerdict = Verdict.NotSet;
            UpgradeVerdict(MyVerdict);
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            if (Action == SetAction.Command)
            {
                myInstrument.Source.SetCurrentLevelTriggeredAmplitude((_currentCPCustomInput ? _currentCPStr : _currentCP.ToString()));

            }
            else
            {
                _currentQR = myInstrument.Source.GetCurrentLevelTriggeredAmplitude((_presetQPCustomInput ? _presetQPStr : _presetQP.ToString()));

                double? result = _currentQR;

                if ((result > LowerLimit && result < UpperLimit) && VerdictTest == VerdictTestEnum.InBetween)
                { MyVerdict = Verdict.Pass; }
                else if ((result == ValueEqualTo) && VerdictTest == VerdictTestEnum.EqualTo)
                { MyVerdict = Verdict.Pass; }
                else if ((result >= LowerLimit) && VerdictTest == VerdictTestEnum.GreaterThan)
                { MyVerdict = Verdict.Pass; }
                else if ((result <= UpperLimit) && VerdictTest == VerdictTestEnum.LessThan)
                { MyVerdict = Verdict.Pass; }
                else if (VerdictTest == VerdictTestEnum.Ignore)
                { MyVerdict = Verdict.Pass; }
                else
                { MyVerdict = Verdict.Fail; }
                UpgradeVerdict(MyVerdict);
                if (publishResults)
                {
                    Results.Publish("current", new { Current = (double)_currentQR });
                }
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xSourceVoltageRangeStep
    [Display("RANGe", Groups: new[] { "E364x", "[SOURce]", "VOLTage" }, Description: "Select an output range to be programmed by the identifier.  Query the currently selected range.")]
    public class E364xSourceVoltageRangeStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_31.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "range Custom Input", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public bool _rangeCPCustomInput { get; set; }
        [Display(Group: "Command Parameter ", Name: "range", Description: "The range.", Order: 30.2)]
        [EnabledIf("_rangeCPCustomInput", false, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public output _rangeCP { get; set; }
        [Display(Group: "Command Parameter ", Name: "range", Description: "The range.", Order: 30.3)]
        [EnabledIf("_rangeCPCustomInput", true, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public string _rangeCPStr { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "range", Description: "Returns the currently selected range.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public output _rangeQR { get; private set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xSourceVoltageRangeStep()
        {
            {
                Name = "[SOURce]:VOLTage:RANGe";
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            if (Action == SetAction.Command)
            {
                myInstrument.Source.SetVoltageRange((_rangeCPCustomInput ? _rangeCPStr : _rangeCP.ToString()));

            }
            else
            {
                _rangeQR = myInstrument.Source.GetVoltageRange();
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xSourceVoltageProtectionClearStep
    [Display("CLEar", Groups: new[] { "E364x", "[SOURce]", "VOLTage", "PROTection" }, Description: "Cause the overvoltage protection circuit to be cleared.")]
    public class E364xSourceVoltageProtectionClearStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_30.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xSourceVoltageProtectionClearStep()
        {
            {
                Name = "[SOURce]:VOLTage:PROTection:CLEar";
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            myInstrument.Source.SetVoltageProtectionClear();
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xSourceVoltageProtectionLevelStep
    [Display("[LEVel]", Groups: new[] { "E364x", "[SOURce]", "VOLTage", "PROTection" }, Description: "Set the voltage level at which the overvoltage protection (OVP) circuit will trip.   Query the overvoltage protection trip level presently programmed.")]
    public class E364xSourceVoltageProtectionLevelStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_25.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "voltage Custom Input", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public bool _voltageCPCustomInput { get; set; }
        [Display(Group: "Command Parameter ", Name: "voltage", Description: "The voltage level at which the overvoltage protection (OVP) circuit will trip.", Order: 30.2)]
        [EnabledIf("_voltageCPCustomInput", false, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public minMax _voltageCP { get; set; }
        [Display(Group: "Command Parameter ", Name: "voltage", Description: "The voltage level at which the overvoltage protection (OVP) circuit will trip.", Order: 30.3)]
        [EnabledIf("_voltageCPCustomInput", true, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public string _voltageCPStr { get; set; }
        [Display(Group: "Query Parameter ", Name: "preset Custom Input", Order: 40.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public bool _presetQPCustomInput { get; set; }
        [Display(Group: "Query Parameter ", Name: "preset", Description: "MINimum|MAXimum", Order: 40.2)]
        [EnabledIf("_presetQPCustomInput", false, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public minMax _presetQP { get; set; }
        [Display(Group: "Query Parameter ", Name: "preset", Description: "MINimum|MAXimum", Order: 40.3)]
        [EnabledIf("_presetQPCustomInput", true, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public string _presetQPStr { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "voltage", Description: "Returns the current value of the voltage level at which the overvoltage protection (OVP) circuit will trip.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public double? _voltageQR { get; private set; }
        #endregion
        #region Result Checkbox
        [Display("Publish Results", Group: "Results", Description: "Enable to publish results", Order: 58.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool publishResults { get; set; }
        #endregion
        #region Verdict Properties
        [Output]
        [Display("Verdict", Group: "Verdict", Order: 60.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public Verdict MyVerdict { get; private set; }
        public enum VerdictTestEnum { EqualTo, LessThan, GreaterThan, InBetween, Ignore };
        [Display(Group: "Numeric Limit Test", Name: "Numeric Limit Test", Order: 61.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public VerdictTestEnum VerdictTest { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Value", Order: 61.2, Description: "The value allowed. If not equal, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.EqualTo, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public double ValueEqualTo { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Lower Limit Value", Order: 61.3, Description: "The minimum value allowed. If less, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.GreaterThan, VerdictTestEnum.InBetween, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public double LowerLimit { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Upper Limit Value", Order: 61.4, Description: "The maximum value allowed. If exceeded, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.LessThan, VerdictTestEnum.InBetween, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public double UpperLimit { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xSourceVoltageProtectionLevelStep()
        {
            {
                Name = "[SOURce]:VOLTage:PROTection:[LEVel]";
                VerdictTest = VerdictTestEnum.Ignore;
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            MyVerdict = Verdict.NotSet;
            UpgradeVerdict(MyVerdict);
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            if (Action == SetAction.Command)
            {
                myInstrument.Source.SetVoltageProtectionLevel((_voltageCPCustomInput ? _voltageCPStr : _voltageCP.ToString()));

            }
            else
            {
                _voltageQR = myInstrument.Source.GetVoltageProtectionLevel((_presetQPCustomInput ? _presetQPStr : _presetQP.ToString()));

                double? result = _voltageQR;

                if ((result > LowerLimit && result < UpperLimit) && VerdictTest == VerdictTestEnum.InBetween)
                { MyVerdict = Verdict.Pass; }
                else if ((result == ValueEqualTo) && VerdictTest == VerdictTestEnum.EqualTo)
                { MyVerdict = Verdict.Pass; }
                else if ((result >= LowerLimit) && VerdictTest == VerdictTestEnum.GreaterThan)
                { MyVerdict = Verdict.Pass; }
                else if ((result <= UpperLimit) && VerdictTest == VerdictTestEnum.LessThan)
                { MyVerdict = Verdict.Pass; }
                else if (VerdictTest == VerdictTestEnum.Ignore)
                { MyVerdict = Verdict.Pass; }
                else
                { MyVerdict = Verdict.Fail; }
                UpgradeVerdict(MyVerdict);
                if (publishResults)
                {
                    Results.Publish("voltage", new { Voltage = (double)_voltageQR });
                }
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xSourceVoltageProtectionStateStep
    [Display("STATe", Groups: new[] { "E364x", "[SOURce]", "VOLTage", "PROTection" }, Description: "Enable or disable the overvoltage protection function.  Query the state of the overvoltage protection function.")]
    public class E364xSourceVoltageProtectionStateStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_27.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "state", Description: "Enable/disable the overvoltage protection function.", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public bool _stateCP { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "state", Description: "Returns the current value of the overvoltage protection function.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public bool _stateQR { get; private set; }
        #endregion
        #region Verdict Properties
        [Output]
        [Display("Verdict", Group: "Verdict", Order: 60.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public Verdict MyVerdict { get; private set; }
        public enum VerdictTestEnum { EqualTo, Ignore };
        [Display(Group: "Boolean Test", Name: "Boolean Test", Order: 61.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public VerdictTestEnum VerdictTest { get; set; }

        [Display(Group: "Boolean Test", Name: "Test Value", Order: 61.2)]
        [EnabledIf("VerdictTest", VerdictTestEnum.EqualTo, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool TestValue { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xSourceVoltageProtectionStateStep()
        {
            {
                Name = "[SOURce]:VOLTage:PROTection:STATe";
                VerdictTest = VerdictTestEnum.Ignore;
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            MyVerdict = Verdict.NotSet;
            UpgradeVerdict(MyVerdict);
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            if (Action == SetAction.Command)
            {
                myInstrument.Source.SetVoltageProtectionState(_stateCP);

            }
            else
            {
                _stateQR = myInstrument.Source.GetVoltageProtectionState();

                bool result = _stateQR;

                if (result == TestValue && VerdictTest == VerdictTestEnum.EqualTo)
                { MyVerdict = Verdict.Pass; }
                else if (VerdictTest == VerdictTestEnum.Ignore)
                { MyVerdict = Verdict.Pass; }
                else { MyVerdict = Verdict.Fail; }
                UpgradeVerdict(MyVerdict);
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xSourceVoltageProtectionTrippedStep
    [Display("TRIPped", Groups: new[] { "E364x", "[SOURce]", "VOLTage", "PROTection" }, Description: "Return a ‘‘1’’ if the overvoltage protection circuit is tripped and not cleared or a ‘‘0’’ if not tripped.")]
    public class E364xSourceVoltageProtectionTrippedStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_29.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Output]
        [Display(Group: "Query Response ", Name: "tripped", Description: "Returns the current value of the function.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public bool _trippedQR { get; private set; }
        #endregion
        #region Verdict Properties
        [Output]
        [Display("Verdict", Group: "Verdict", Order: 60.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public Verdict MyVerdict { get; private set; }
        public enum VerdictTestEnum { EqualTo, Ignore };
        [Display(Group: "Boolean Test", Name: "Boolean Test", Order: 61.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public VerdictTestEnum VerdictTest { get; set; }

        [Display(Group: "Boolean Test", Name: "Test Value", Order: 61.2)]
        [EnabledIf("VerdictTest", VerdictTestEnum.EqualTo, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool TestValue { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xSourceVoltageProtectionTrippedStep()
        {
            {
                Name = "[SOURce]:VOLTage:PROTection:TRIPped";
                VerdictTest = VerdictTestEnum.Ignore;
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            MyVerdict = Verdict.NotSet;
            UpgradeVerdict(MyVerdict);
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            _trippedQR = myInstrument.Source.GetVoltageProtectionTripped();

            bool result = _trippedQR;

            if (result == TestValue && VerdictTest == VerdictTestEnum.EqualTo)
            { MyVerdict = Verdict.Pass; }
            else if (VerdictTest == VerdictTestEnum.Ignore)
            { MyVerdict = Verdict.Pass; }
            else { MyVerdict = Verdict.Fail; }
            UpgradeVerdict(MyVerdict);
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xSourceVoltageLevelImmediateAmplitudeStep
    [Display("[AMPLitude]", Groups: new[] { "E364x", "[SOURce]", "VOLTage", "[LEVel]", "[IMMediate]" }, Description: "Program the immediate voltage level of the power supply.   Query the presently programmed voltage level of the power supply.")]
    public class E364xSourceVoltageLevelImmediateAmplitudeStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_19.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "voltage Custom Input", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public bool _voltageCPCustomInput { get; set; }
        [Display(Group: "Command Parameter ", Name: "voltage", Description: "The immediate voltage level of the power supply.", Order: 30.2)]
        [EnabledIf("_voltageCPCustomInput", false, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public minMaxUpDown _voltageCP { get; set; }
        [Display(Group: "Command Parameter ", Name: "voltage", Description: "The immediate voltage level of the power supply.", Order: 30.3)]
        [EnabledIf("_voltageCPCustomInput", true, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public string _voltageCPStr { get; set; }
        [Display(Group: "Query Parameter ", Name: "preset Custom Input", Order: 40.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public bool _presetQPCustomInput { get; set; }
        [Display(Group: "Query Parameter ", Name: "preset", Description: "MINimum | MAXimum | UP | DOWN", Order: 40.2)]
        [EnabledIf("_presetQPCustomInput", false, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public minMax _presetQP { get; set; }
        [Display(Group: "Query Parameter ", Name: "preset", Description: "MINimum | MAXimum | UP | DOWN", Order: 40.3)]
        [EnabledIf("_presetQPCustomInput", true, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public string _presetQPStr { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "voltage", Description: "Returnsthe presently programmed voltage level of the power supply.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public double? _voltageQR { get; private set; }
        #endregion
        #region Result Checkbox
        [Display("Publish Results", Group: "Results", Description: "Enable to publish results", Order: 58.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool publishResults { get; set; }
        #endregion
        #region Verdict Properties
        [Output]
        [Display("Verdict", Group: "Verdict", Order: 60.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public Verdict MyVerdict { get; private set; }
        public enum VerdictTestEnum { EqualTo, LessThan, GreaterThan, InBetween, Ignore };
        [Display(Group: "Numeric Limit Test", Name: "Numeric Limit Test", Order: 61.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public VerdictTestEnum VerdictTest { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Value", Order: 61.2, Description: "The value allowed. If not equal, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.EqualTo, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public double ValueEqualTo { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Lower Limit Value", Order: 61.3, Description: "The minimum value allowed. If less, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.GreaterThan, VerdictTestEnum.InBetween, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public double LowerLimit { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Upper Limit Value", Order: 61.4, Description: "The maximum value allowed. If exceeded, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.LessThan, VerdictTestEnum.InBetween, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public double UpperLimit { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xSourceVoltageLevelImmediateAmplitudeStep()
        {
            {
                Name = "[SOURce]:VOLTage:[LEVel]:[IMMediate]:[AMPLitude]";
                VerdictTest = VerdictTestEnum.Ignore;
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            MyVerdict = Verdict.NotSet;
            UpgradeVerdict(MyVerdict);
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            if (Action == SetAction.Command)
            {
                myInstrument.Source.SetVoltageLevelImmediateAmplitude((_voltageCPCustomInput ? _voltageCPStr : _voltageCP.ToString()));

            }
            else
            {
                _voltageQR = myInstrument.Source.GetVoltageLevelImmediateAmplitude((_presetQPCustomInput ? _presetQPStr : _presetQP.ToString()));

                double? result = _voltageQR;

                if ((result > LowerLimit && result < UpperLimit) && VerdictTest == VerdictTestEnum.InBetween)
                { MyVerdict = Verdict.Pass; }
                else if ((result == ValueEqualTo) && VerdictTest == VerdictTestEnum.EqualTo)
                { MyVerdict = Verdict.Pass; }
                else if ((result >= LowerLimit) && VerdictTest == VerdictTestEnum.GreaterThan)
                { MyVerdict = Verdict.Pass; }
                else if ((result <= UpperLimit) && VerdictTest == VerdictTestEnum.LessThan)
                { MyVerdict = Verdict.Pass; }
                else if (VerdictTest == VerdictTestEnum.Ignore)
                { MyVerdict = Verdict.Pass; }
                else
                { MyVerdict = Verdict.Fail; }
                UpgradeVerdict(MyVerdict);
                if (publishResults)
                {
                    Results.Publish("voltage", new { Voltage = (double)_voltageQR });
                }
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xSourceVoltageLevelImmediateStepIncrementStep
    [Display("[INCRement]", Groups: new[] { "E364x", "[SOURce]", "VOLTage", "[LEVel]", "[IMMediate]", "STEP" }, Description: "Set the step size for voltage programming with the VOLT UP and VOLT DOWN commands.  Return the value of the step size currently specified.")]
    public class E364xSourceVoltageLevelImmediateStepIncrementStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_21.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "numericValue Custom Input", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public bool _numericValueCPCustomInput { get; set; }
        [Display(Group: "Command Parameter ", Name: "numericValue", Description: "The value of the step size currently specified.", Order: 30.2)]
        [EnabledIf("_numericValueCPCustomInput", false, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public _default _numericValueCP { get; set; }
        [Display(Group: "Command Parameter ", Name: "numericValue", Description: "The value of the step size currently specified.", Order: 30.3)]
        [EnabledIf("_numericValueCPCustomInput", true, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public string _numericValueCPStr { get; set; }
        [Display(Group: "Query Parameter ", Name: "default Custom Input", Order: 40.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public bool _defaultQPCustomInput { get; set; }
        [Display(Group: "Query Parameter ", Name: "default", Description: "Default.", Order: 40.2)]
        [EnabledIf("_defaultQPCustomInput", false, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public _default _defaultQP { get; set; }
        [Display(Group: "Query Parameter ", Name: "default", Description: "Default.", Order: 40.3)]
        [EnabledIf("_defaultQPCustomInput", true, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public string _defaultQPStr { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "numericValue", Description: "Returns the current  value of the step size currently specified.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public double? _numericValueQR { get; private set; }
        #endregion
        #region Result Checkbox
        [Display("Publish Results", Group: "Results", Description: "Enable to publish results", Order: 58.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool publishResults { get; set; }
        #endregion
        #region Verdict Properties
        [Output]
        [Display("Verdict", Group: "Verdict", Order: 60.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public Verdict MyVerdict { get; private set; }
        public enum VerdictTestEnum { EqualTo, LessThan, GreaterThan, InBetween, Ignore };
        [Display(Group: "Numeric Limit Test", Name: "Numeric Limit Test", Order: 61.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public VerdictTestEnum VerdictTest { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Value", Order: 61.2, Description: "The value allowed. If not equal, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.EqualTo, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public double ValueEqualTo { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Lower Limit Value", Order: 61.3, Description: "The minimum value allowed. If less, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.GreaterThan, VerdictTestEnum.InBetween, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public double LowerLimit { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Upper Limit Value", Order: 61.4, Description: "The maximum value allowed. If exceeded, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.LessThan, VerdictTestEnum.InBetween, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public double UpperLimit { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xSourceVoltageLevelImmediateStepIncrementStep()
        {
            {
                Name = "[SOURce]:VOLTage:[LEVel]:[IMMediate]:STEP:[INCRement]";
                VerdictTest = VerdictTestEnum.Ignore;
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            MyVerdict = Verdict.NotSet;
            UpgradeVerdict(MyVerdict);
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            if (Action == SetAction.Command)
            {
                myInstrument.Source.SetVoltageLevelImmediateStepIncrement((_numericValueCPCustomInput ? _numericValueCPStr : _numericValueCP.ToString()));

            }
            else
            {
                _numericValueQR = myInstrument.Source.GetVoltageLevelImmediateStepIncrement((_defaultQPCustomInput ? _defaultQPStr : _defaultQP.ToString()));

                double? result = _numericValueQR;

                if ((result > LowerLimit && result < UpperLimit) && VerdictTest == VerdictTestEnum.InBetween)
                { MyVerdict = Verdict.Pass; }
                else if ((result == ValueEqualTo) && VerdictTest == VerdictTestEnum.EqualTo)
                { MyVerdict = Verdict.Pass; }
                else if ((result >= LowerLimit) && VerdictTest == VerdictTestEnum.GreaterThan)
                { MyVerdict = Verdict.Pass; }
                else if ((result <= UpperLimit) && VerdictTest == VerdictTestEnum.LessThan)
                { MyVerdict = Verdict.Pass; }
                else if (VerdictTest == VerdictTestEnum.Ignore)
                { MyVerdict = Verdict.Pass; }
                else
                { MyVerdict = Verdict.Fail; }
                UpgradeVerdict(MyVerdict);
                if (publishResults)
                {
                    Results.Publish("numericValue", new { Numericvalue = (double)_numericValueQR });
                }
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xSourceVoltageLevelTriggeredAmplitudeStep
    [Display("[AMPLitude]", Groups: new[] { "E364x", "[SOURce]", "VOLTage", "[LEVel]", "TRIGgered" }, Description: "Program the pending triggered voltage level.   Query the triggered voltage level presently programmed.")]
    public class E364xSourceVoltageLevelTriggeredAmplitudeStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_23.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "voltage Custom Input", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public bool _voltageCPCustomInput { get; set; }
        [Display(Group: "Command Parameter ", Name: "voltage", Description: "The pending triggered voltage level.", Order: 30.2)]
        [EnabledIf("_voltageCPCustomInput", false, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public minMax _voltageCP { get; set; }
        [Display(Group: "Command Parameter ", Name: "voltage", Description: "The pending triggered voltage level.", Order: 30.3)]
        [EnabledIf("_voltageCPCustomInput", true, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public string _voltageCPStr { get; set; }
        [Display(Group: "Query Parameter ", Name: "preset Custom Input", Order: 40.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public bool _presetQPCustomInput { get; set; }
        [Display(Group: "Query Parameter ", Name: "preset", Description: "MINimum | MAXimum", Order: 40.2)]
        [EnabledIf("_presetQPCustomInput", false, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public minMax _presetQP { get; set; }
        [Display(Group: "Query Parameter ", Name: "preset", Description: "MINimum | MAXimum", Order: 40.3)]
        [EnabledIf("_presetQPCustomInput", true, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public string _presetQPStr { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "voltage", Description: "Returns the triggered voltage level presently programmed.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public double? _voltageQR { get; private set; }
        #endregion
        #region Result Checkbox
        [Display("Publish Results", Group: "Results", Description: "Enable to publish results", Order: 58.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool publishResults { get; set; }
        #endregion
        #region Verdict Properties
        [Output]
        [Display("Verdict", Group: "Verdict", Order: 60.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public Verdict MyVerdict { get; private set; }
        public enum VerdictTestEnum { EqualTo, LessThan, GreaterThan, InBetween, Ignore };
        [Display(Group: "Numeric Limit Test", Name: "Numeric Limit Test", Order: 61.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public VerdictTestEnum VerdictTest { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Value", Order: 61.2, Description: "The value allowed. If not equal, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.EqualTo, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public double ValueEqualTo { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Lower Limit Value", Order: 61.3, Description: "The minimum value allowed. If less, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.GreaterThan, VerdictTestEnum.InBetween, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public double LowerLimit { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Upper Limit Value", Order: 61.4, Description: "The maximum value allowed. If exceeded, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.LessThan, VerdictTestEnum.InBetween, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public double UpperLimit { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xSourceVoltageLevelTriggeredAmplitudeStep()
        {
            {
                Name = "[SOURce]:VOLTage:[LEVel]:TRIGgered:[AMPLitude]";
                VerdictTest = VerdictTestEnum.Ignore;
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            MyVerdict = Verdict.NotSet;
            UpgradeVerdict(MyVerdict);
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            if (Action == SetAction.Command)
            {
                myInstrument.Source.SetVoltageLevelTriggeredAmplitude((_voltageCPCustomInput ? _voltageCPStr : _voltageCP.ToString()));

            }
            else
            {
                _voltageQR = myInstrument.Source.GetVoltageLevelTriggeredAmplitude((_presetQPCustomInput ? _presetQPStr : _presetQP.ToString()));

                double? result = _voltageQR;

                if ((result > LowerLimit && result < UpperLimit) && VerdictTest == VerdictTestEnum.InBetween)
                { MyVerdict = Verdict.Pass; }
                else if ((result == ValueEqualTo) && VerdictTest == VerdictTestEnum.EqualTo)
                { MyVerdict = Verdict.Pass; }
                else if ((result >= LowerLimit) && VerdictTest == VerdictTestEnum.GreaterThan)
                { MyVerdict = Verdict.Pass; }
                else if ((result <= UpperLimit) && VerdictTest == VerdictTestEnum.LessThan)
                { MyVerdict = Verdict.Pass; }
                else if (VerdictTest == VerdictTestEnum.Ignore)
                { MyVerdict = Verdict.Pass; }
                else
                { MyVerdict = Verdict.Fail; }
                UpgradeVerdict(MyVerdict);
                if (publishResults)
                {
                    Results.Publish("voltage", new { Voltage = (double)_voltageQR });
                }
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #endregion
    #region STATus Node Classes
    #region E364xStatusQuestionableEnableStep
    [Display("ENABle", Groups: new[] { "E364x", "STATus", "QUEStionable" }, Description: "Enable bits in the Questionable Status Enable register.   Query the Questionable Status Enable register.")]
    public class E364xStatusQuestionableEnableStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_75.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "enable", Description: "The Questionable Status Enable register.", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public int? _enableCP { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "enable", Description: "Returns the Questionable Status Enable register.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public int? _enableQR { get; private set; }
        #endregion
        #region Result Checkbox
        [Display("Publish Results", Group: "Results", Description: "Enable to publish results", Order: 58.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool publishResults { get; set; }
        #endregion
        #region Verdict Properties
        [Output]
        [Display("Verdict", Group: "Verdict", Order: 60.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public Verdict MyVerdict { get; private set; }
        public enum VerdictTestEnum { EqualTo, LessThan, GreaterThan, InBetween, Ignore };
        [Display(Group: "Numeric Limit Test", Name: "Numeric Limit Test", Order: 61.1)]
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
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xStatusQuestionableEnableStep()
        {
            {
                Name = "STATus:QUEStionable:ENABle";
                VerdictTest = VerdictTestEnum.Ignore;
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            MyVerdict = Verdict.NotSet;
            UpgradeVerdict(MyVerdict);
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            if (Action == SetAction.Command)
            {
                myInstrument.Status.SetQuestionableEnable(_enableCP);

            }
            else
            {
                _enableQR = myInstrument.Status.GetQuestionableEnable();

                int? result = _enableQR;

                if ((result > LowerLimit && result < UpperLimit) && VerdictTest == VerdictTestEnum.InBetween)
                { MyVerdict = Verdict.Pass; }
                else if ((result == ValueEqualTo) && VerdictTest == VerdictTestEnum.EqualTo)
                { MyVerdict = Verdict.Pass; }
                else if ((result >= LowerLimit) && VerdictTest == VerdictTestEnum.GreaterThan)
                { MyVerdict = Verdict.Pass; }
                else if ((result <= UpperLimit) && VerdictTest == VerdictTestEnum.LessThan)
                { MyVerdict = Verdict.Pass; }
                else if (VerdictTest == VerdictTestEnum.Ignore)
                { MyVerdict = Verdict.Pass; }
                else
                { MyVerdict = Verdict.Fail; }
                UpgradeVerdict(MyVerdict);
                if (publishResults)
                {
                    Results.Publish("enable", new { Enable = (int)_enableQR });
                }
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xStatusQuestionableEventStep
    [Display("[EVENt]", Groups: new[] { "E364x", "STATus", "QUEStionable" }, Description: "Query the Questionable Status Event register.")]
    public class E364xStatusQuestionableEventStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_74.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Output]
        [Display(Group: "Query Response ", Name: "event", Description: "Returns the Questionable Status Event register.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public int? _eventQR { get; private set; }
        #endregion
        #region Result Checkbox
        [Display("Publish Results", Group: "Results", Description: "Enable to publish results", Order: 58.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool publishResults { get; set; }
        #endregion
        #region Verdict Properties
        [Output]
        [Display("Verdict", Group: "Verdict", Order: 60.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public Verdict MyVerdict { get; private set; }
        public enum VerdictTestEnum { EqualTo, LessThan, GreaterThan, InBetween, Ignore };
        [Display(Group: "Numeric Limit Test", Name: "Numeric Limit Test", Order: 61.1)]
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
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xStatusQuestionableEventStep()
        {
            {
                Name = "STATus:QUEStionable:[EVENt]";
                VerdictTest = VerdictTestEnum.Ignore;
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            MyVerdict = Verdict.NotSet;
            UpgradeVerdict(MyVerdict);
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            _eventQR = myInstrument.Status.GetQuestionableEvent();

            int? result = _eventQR;

            if ((result > LowerLimit && result < UpperLimit) && VerdictTest == VerdictTestEnum.InBetween)
            { MyVerdict = Verdict.Pass; }
            else if ((result == ValueEqualTo) && VerdictTest == VerdictTestEnum.EqualTo)
            { MyVerdict = Verdict.Pass; }
            else if ((result >= LowerLimit) && VerdictTest == VerdictTestEnum.GreaterThan)
            { MyVerdict = Verdict.Pass; }
            else if ((result <= UpperLimit) && VerdictTest == VerdictTestEnum.LessThan)
            { MyVerdict = Verdict.Pass; }
            else if (VerdictTest == VerdictTestEnum.Ignore)
            { MyVerdict = Verdict.Pass; }
            else
            { MyVerdict = Verdict.Fail; }
            UpgradeVerdict(MyVerdict);
            if (publishResults)
            {
                Results.Publish("event", new { Event = (int)_eventQR });
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xStatusQuestionableInstrumentEventStep
    [Display("[EVENt]", Groups: new[] { "E364x", "STATus", "QUEStionable", "INSTrument" }, Description: "Query the Questionable Instrument Event register.")]
    public class E364xStatusQuestionableInstrumentEventStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_77.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Output]
        [Display(Group: "Query Response ", Name: "event", Description: "Returns the Questionable Instrument Event register.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public int? _eventQR { get; private set; }
        #endregion
        #region Result Checkbox
        [Display("Publish Results", Group: "Results", Description: "Enable to publish results", Order: 58.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool publishResults { get; set; }
        #endregion
        #region Verdict Properties
        [Output]
        [Display("Verdict", Group: "Verdict", Order: 60.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public Verdict MyVerdict { get; private set; }
        public enum VerdictTestEnum { EqualTo, LessThan, GreaterThan, InBetween, Ignore };
        [Display(Group: "Numeric Limit Test", Name: "Numeric Limit Test", Order: 61.1)]
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
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xStatusQuestionableInstrumentEventStep()
        {
            {
                Name = "STATus:QUEStionable:INSTrument:[EVENt]";
                VerdictTest = VerdictTestEnum.Ignore;
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            MyVerdict = Verdict.NotSet;
            UpgradeVerdict(MyVerdict);
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            _eventQR = myInstrument.Status.GetQuestionableInstrumentEvent();

            int? result = _eventQR;

            if ((result > LowerLimit && result < UpperLimit) && VerdictTest == VerdictTestEnum.InBetween)
            { MyVerdict = Verdict.Pass; }
            else if ((result == ValueEqualTo) && VerdictTest == VerdictTestEnum.EqualTo)
            { MyVerdict = Verdict.Pass; }
            else if ((result >= LowerLimit) && VerdictTest == VerdictTestEnum.GreaterThan)
            { MyVerdict = Verdict.Pass; }
            else if ((result <= UpperLimit) && VerdictTest == VerdictTestEnum.LessThan)
            { MyVerdict = Verdict.Pass; }
            else if (VerdictTest == VerdictTestEnum.Ignore)
            { MyVerdict = Verdict.Pass; }
            else
            { MyVerdict = Verdict.Fail; }
            UpgradeVerdict(MyVerdict);
            if (publishResults)
            {
                Results.Publish("event", new { Event = (int)_eventQR });
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xStatusQuestionableInstrumentEnableStep
    [Display("ENABle", Groups: new[] { "E364x", "STATus", "QUEStionable", "INSTrument" }, Description: "Set the value of the Questionable Instrument Enable register.  Return the value of the Questionable Instrument Enable register.")]
    public class E364xStatusQuestionableInstrumentEnableStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_78.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "enableValue", Description: "The value of the Questionable Instrument Enable register.", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public int? _enableValueCP { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "enable", Description: "Return the value of the Questionable Instrument Enable register.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public int? _enableQR { get; private set; }
        #endregion
        #region Result Checkbox
        [Display("Publish Results", Group: "Results", Description: "Enable to publish results", Order: 58.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool publishResults { get; set; }
        #endregion
        #region Verdict Properties
        [Output]
        [Display("Verdict", Group: "Verdict", Order: 60.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public Verdict MyVerdict { get; private set; }
        public enum VerdictTestEnum { EqualTo, LessThan, GreaterThan, InBetween, Ignore };
        [Display(Group: "Numeric Limit Test", Name: "Numeric Limit Test", Order: 61.1)]
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
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xStatusQuestionableInstrumentEnableStep()
        {
            {
                Name = "STATus:QUEStionable:INSTrument:ENABle";
                VerdictTest = VerdictTestEnum.Ignore;
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            MyVerdict = Verdict.NotSet;
            UpgradeVerdict(MyVerdict);
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            if (Action == SetAction.Command)
            {
                myInstrument.Status.SetQuestionableInstrumentEnable(_enableValueCP);

            }
            else
            {
                _enableQR = myInstrument.Status.GetQuestionableInstrumentEnable();

                int? result = _enableQR;

                if ((result > LowerLimit && result < UpperLimit) && VerdictTest == VerdictTestEnum.InBetween)
                { MyVerdict = Verdict.Pass; }
                else if ((result == ValueEqualTo) && VerdictTest == VerdictTestEnum.EqualTo)
                { MyVerdict = Verdict.Pass; }
                else if ((result >= LowerLimit) && VerdictTest == VerdictTestEnum.GreaterThan)
                { MyVerdict = Verdict.Pass; }
                else if ((result <= UpperLimit) && VerdictTest == VerdictTestEnum.LessThan)
                { MyVerdict = Verdict.Pass; }
                else if (VerdictTest == VerdictTestEnum.Ignore)
                { MyVerdict = Verdict.Pass; }
                else
                { MyVerdict = Verdict.Fail; }
                UpgradeVerdict(MyVerdict);
                if (publishResults)
                {
                    Results.Publish("enable", new { Enable = (int)_enableQR });
                }
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xStatusQuestionableInstrumentIsummaryConditionStep
    [Display("CONDition", Groups: new[] { "E364x", "STATus", "QUEStionable", "INSTrument", "ISUMmary" }, Description: "Return the CV or CC condition of the specified instrument.")]
    public class E364xStatusQuestionableInstrumentIsummaryConditionStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_81.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Settings", Name: "ISUMmary <n>", Description: "output identifier. (min: 1, max: 2)", Order: 25.1)]
        public int? ISUMmarySuffix { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "condition", Description: "Return the CV or CC condition of the specified instrument.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public int? _conditionQR { get; private set; }
        #endregion
        #region Result Checkbox
        [Display("Publish Results", Group: "Results", Description: "Enable to publish results", Order: 58.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool publishResults { get; set; }
        #endregion
        #region Verdict Properties
        [Output]
        [Display("Verdict", Group: "Verdict", Order: 60.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public Verdict MyVerdict { get; private set; }
        public enum VerdictTestEnum { EqualTo, LessThan, GreaterThan, InBetween, Ignore };
        [Display(Group: "Numeric Limit Test", Name: "Numeric Limit Test", Order: 61.1)]
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
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xStatusQuestionableInstrumentIsummaryConditionStep()
        {
            {
                Name = "STATus:QUEStionable:INSTrument:ISUMmary:CONDition";
                VerdictTest = VerdictTestEnum.Ignore;
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            MyVerdict = Verdict.NotSet;
            UpgradeVerdict(MyVerdict);
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            _conditionQR = myInstrument.Status.GetQuestionableInstrumentIsummaryCondition(ISUMmarySuffix);

            int? result = _conditionQR;

            if ((result > LowerLimit && result < UpperLimit) && VerdictTest == VerdictTestEnum.InBetween)
            { MyVerdict = Verdict.Pass; }
            else if ((result == ValueEqualTo) && VerdictTest == VerdictTestEnum.EqualTo)
            { MyVerdict = Verdict.Pass; }
            else if ((result >= LowerLimit) && VerdictTest == VerdictTestEnum.GreaterThan)
            { MyVerdict = Verdict.Pass; }
            else if ((result <= UpperLimit) && VerdictTest == VerdictTestEnum.LessThan)
            { MyVerdict = Verdict.Pass; }
            else if (VerdictTest == VerdictTestEnum.Ignore)
            { MyVerdict = Verdict.Pass; }
            else
            { MyVerdict = Verdict.Fail; }
            UpgradeVerdict(MyVerdict);
            if (publishResults)
            {
                Results.Publish("condition", new { Condition = (int)_conditionQR });
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xStatusQuestionableInstrumentIsummaryEventStep
    [Display("[EVENt]", Groups: new[] { "E364x", "STATus", "QUEStionable", "INSTrument", "ISUMmary" }, Description: "Return the value of the Questionable Instrument Isummary Event register for a specific output of the two-output power supply.")]
    public class E364xStatusQuestionableInstrumentIsummaryEventStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_80.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Settings", Name: "ISUMmary <n>", Description: "output identifier. (min: 1, max: 2)", Order: 25.1)]
        public int? ISUMmarySuffix { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "event", Description: "Return the value of the Questionable Instrument Isummary Event register for a specific output of the two-output power supply.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public int? _eventQR { get; private set; }
        #endregion
        #region Result Checkbox
        [Display("Publish Results", Group: "Results", Description: "Enable to publish results", Order: 58.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool publishResults { get; set; }
        #endregion
        #region Verdict Properties
        [Output]
        [Display("Verdict", Group: "Verdict", Order: 60.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public Verdict MyVerdict { get; private set; }
        public enum VerdictTestEnum { EqualTo, LessThan, GreaterThan, InBetween, Ignore };
        [Display(Group: "Numeric Limit Test", Name: "Numeric Limit Test", Order: 61.1)]
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
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xStatusQuestionableInstrumentIsummaryEventStep()
        {
            {
                Name = "STATus:QUEStionable:INSTrument:ISUMmary:[EVENt]";
                VerdictTest = VerdictTestEnum.Ignore;
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            MyVerdict = Verdict.NotSet;
            UpgradeVerdict(MyVerdict);
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            _eventQR = myInstrument.Status.GetQuestionableInstrumentIsummaryEvent(ISUMmarySuffix);

            int? result = _eventQR;

            if ((result > LowerLimit && result < UpperLimit) && VerdictTest == VerdictTestEnum.InBetween)
            { MyVerdict = Verdict.Pass; }
            else if ((result == ValueEqualTo) && VerdictTest == VerdictTestEnum.EqualTo)
            { MyVerdict = Verdict.Pass; }
            else if ((result >= LowerLimit) && VerdictTest == VerdictTestEnum.GreaterThan)
            { MyVerdict = Verdict.Pass; }
            else if ((result <= UpperLimit) && VerdictTest == VerdictTestEnum.LessThan)
            { MyVerdict = Verdict.Pass; }
            else if (VerdictTest == VerdictTestEnum.Ignore)
            { MyVerdict = Verdict.Pass; }
            else
            { MyVerdict = Verdict.Fail; }
            UpgradeVerdict(MyVerdict);
            if (publishResults)
            {
                Results.Publish("event", new { Event = (int)_eventQR });
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xStatusQuestionableInstrumentIsummaryEnableStep
    [Display("ENABle", Groups: new[] { "E364x", "STATus", "QUEStionable", "INSTrument", "ISUMmary" }, Description: "Set the value of the Questionable Instrument Isummary Enable register for a specific output of the two-output power supply.   This query returns the value of the Questionable Instrument Isummary Enable register.")]
    public class E364xStatusQuestionableInstrumentIsummaryEnableStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_82.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Settings", Name: "ISUMmary <n>", Description: "output identifier. (min: 1, max: 2)", Order: 25.1)]
        public int? ISUMmarySuffix { get; set; }
        [Display(Group: "Command Parameter ", Name: "enableValue", Description: "The value of the Questionable Instrument Isummary Enable register.", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public int? _enableValueCP { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "enableValue", Description: "Returns the current value of the Questionable Instrument Isummary Enable register.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public int? _enableValueQR { get; private set; }
        #endregion
        #region Result Checkbox
        [Display("Publish Results", Group: "Results", Description: "Enable to publish results", Order: 58.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool publishResults { get; set; }
        #endregion
        #region Verdict Properties
        [Output]
        [Display("Verdict", Group: "Verdict", Order: 60.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public Verdict MyVerdict { get; private set; }
        public enum VerdictTestEnum { EqualTo, LessThan, GreaterThan, InBetween, Ignore };
        [Display(Group: "Numeric Limit Test", Name: "Numeric Limit Test", Order: 61.1)]
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
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xStatusQuestionableInstrumentIsummaryEnableStep()
        {
            {
                Name = "STATus:QUEStionable:INSTrument:ISUMmary:ENABle";
                VerdictTest = VerdictTestEnum.Ignore;
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            MyVerdict = Verdict.NotSet;
            UpgradeVerdict(MyVerdict);
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            if (Action == SetAction.Command)
            {
                myInstrument.Status.SetQuestionableInstrumentIsummaryEnable(ISUMmarySuffix, _enableValueCP);

            }
            else
            {
                _enableValueQR = myInstrument.Status.GetQuestionableInstrumentIsummaryEnable(ISUMmarySuffix);

                int? result = _enableValueQR;

                if ((result > LowerLimit && result < UpperLimit) && VerdictTest == VerdictTestEnum.InBetween)
                { MyVerdict = Verdict.Pass; }
                else if ((result == ValueEqualTo) && VerdictTest == VerdictTestEnum.EqualTo)
                { MyVerdict = Verdict.Pass; }
                else if ((result >= LowerLimit) && VerdictTest == VerdictTestEnum.GreaterThan)
                { MyVerdict = Verdict.Pass; }
                else if ((result <= UpperLimit) && VerdictTest == VerdictTestEnum.LessThan)
                { MyVerdict = Verdict.Pass; }
                else if (VerdictTest == VerdictTestEnum.Ignore)
                { MyVerdict = Verdict.Pass; }
                else
                { MyVerdict = Verdict.Fail; }
                UpgradeVerdict(MyVerdict);
                if (publishResults)
                {
                    Results.Publish("enableValue", new { Enablevalue = (int)_enableValueQR });
                }
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #endregion
    #region SYSTem Node Classes
    #region E364xSystemBeeperImmediateStep
    [Display("[IMMediate]", Groups: new[] { "E364x", "SYSTem", "BEEPer" }, Description: "Issue a single beep immediately.")]
    public class E364xSystemBeeperImmediateStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_50.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xSystemBeeperImmediateStep()
        {
            {
                Name = "SYSTem:BEEPer:[IMMediate]";
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            myInstrument._System.SetBeeperImmediate();
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xSystemErrorStep
    [Display("ERRor", Groups: new[] { "E364x", "SYSTem" }, Description: "Query the power supply’s error queue.")]
    public class E364xSystemErrorStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_51.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Output]
        [Display(Group: "Query Response ", Name: "erorr", Description: "Erorr numbers.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public int? _erorrQR { get; private set; }
        [Output]
        [Display(Group: "Query Response ", Name: "message", Description: "Messages.", Order: 50.2)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public string _messageQR { get; private set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xSystemErrorStep()
        {
            {
                Name = "SYSTem:ERRor";
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            (_erorrQR, _messageQR) = myInstrument._System.GetError();
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xSystemInterfaceStep
    [Display("INTerface", Groups: new[] { "E364x", "SYSTem" }, Description: "Select the remote interface.")]
    public class E364xSystemInterfaceStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_70.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "interface Custom Input", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public bool _interfaceCPCustomInput { get; set; }
        [Display(Group: "Command Parameter ", Name: "interface", Description: "The remote interface.", Order: 30.2)]
        [EnabledIf("_interfaceCPCustomInput", false, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public gpibRs232 _interfaceCP { get; set; }
        [Display(Group: "Command Parameter ", Name: "interface", Description: "The remote interface.", Order: 30.3)]
        [EnabledIf("_interfaceCPCustomInput", true, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public string _interfaceCPStr { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xSystemInterfaceStep()
        {
            {
                Name = "SYSTem:INTerface";
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            myInstrument._System.SetInterface((_interfaceCPCustomInput ? _interfaceCPStr : _interfaceCP.ToString()));
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xSystemLocalStep
    [Display("LOCal", Groups: new[] { "E364x", "SYSTem" }, Description: "Place the power supply in the local mode during RS-232 operation.")]
    public class E364xSystemLocalStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_71.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xSystemLocalStep()
        {
            {
                Name = "SYSTem:LOCal";
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            myInstrument._System.SetLocal();
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xSystemRemoteStep
    [Display("REMote", Groups: new[] { "E364x", "SYSTem" }, Description: "Place the power supply in the remote mode for RS-232 operation.")]
    public class E364xSystemRemoteStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_72.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xSystemRemoteStep()
        {
            {
                Name = "SYSTem:REMote";
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            myInstrument._System.SetRemote();
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xSystemRwlockStep
    [Display("RWLock", Groups: new[] { "E364x", "SYSTem" }, Description: "Place the power supply in the remote mode for RS-232 operation.")]
    public class E364xSystemRwlockStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_73.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xSystemRwlockStep()
        {
            {
                Name = "SYSTem:RWLock";
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            myInstrument._System.SetRwlock();
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xSystemVersionStep
    [Display("VERSion", Groups: new[] { "E364x", "SYSTem" }, Description: "Query the power supply to determine the present SCPI version.")]
    public class E364xSystemVersionStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_52.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Output]
        [Display(Group: "Query Response ", Name: "version", Description: "Returns the power supply to determine the present SCPI version.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public string _versionQR { get; private set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xSystemVersionStep()
        {
            {
                Name = "SYSTem:VERSion";
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            _versionQR = myInstrument._System.GetVersion();
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #endregion
    #region TRIGger Node Classes
    #region E364xTriggerSequenceDelayStep
    [Display("DELay", Groups: new[] { "E364x", "TRIGger", "[SEQuence]" }, Description: "Set the time delay between the detection of an event on the specified trigger source and the start of any corresponding trigger action on the power supply output.  Query the trigger delay.")]
    public class E364xTriggerSequenceDelayStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_34.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "seconds Custom Input", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public bool _secondsCPCustomInput { get; set; }
        [Display(Group: "Command Parameter ", Name: "seconds", Description: "The trigger delay.", Order: 30.2)]
        [EnabledIf("_secondsCPCustomInput", false, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public minMax _secondsCP { get; set; }
        [Display(Group: "Command Parameter ", Name: "seconds", Description: "The trigger delay.", Order: 30.3)]
        [EnabledIf("_secondsCPCustomInput", true, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public string _secondsCPStr { get; set; }
        [Display(Group: "Query Parameter ", Name: "preset Custom Input", Order: 40.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public bool _presetQPCustomInput { get; set; }
        [Display(Group: "Query Parameter ", Name: "preset", Description: "MINimum | MAXimum", Order: 40.2)]
        [EnabledIf("_presetQPCustomInput", false, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public minMax _presetQP { get; set; }
        [Display(Group: "Query Parameter ", Name: "preset", Description: "MINimum | MAXimum", Order: 40.3)]
        [EnabledIf("_presetQPCustomInput", true, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public string _presetQPStr { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "seconds", Description: "Returns the current value of the trigger delay.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public double? _secondsQR { get; private set; }
        #endregion
        #region Result Checkbox
        [Display("Publish Results", Group: "Results", Description: "Enable to publish results", Order: 58.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool publishResults { get; set; }
        #endregion
        #region Verdict Properties
        [Output]
        [Display("Verdict", Group: "Verdict", Order: 60.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public Verdict MyVerdict { get; private set; }
        public enum VerdictTestEnum { EqualTo, LessThan, GreaterThan, InBetween, Ignore };
        [Display(Group: "Numeric Limit Test", Name: "Numeric Limit Test", Order: 61.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public VerdictTestEnum VerdictTest { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Value", Order: 61.2, Description: "The value allowed. If not equal, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.EqualTo, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public double ValueEqualTo { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Lower Limit Value", Order: 61.3, Description: "The minimum value allowed. If less, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.GreaterThan, VerdictTestEnum.InBetween, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public double LowerLimit { get; set; }
        [Display(Group: "Numeric Limit Test", Name: "Upper Limit Value", Order: 61.4, Description: "The maximum value allowed. If exceeded, the test will be marked as failed.")]
        [EnabledIf("VerdictTest", VerdictTestEnum.LessThan, VerdictTestEnum.InBetween, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public double UpperLimit { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xTriggerSequenceDelayStep()
        {
            {
                Name = "TRIGger:[SEQuence]:DELay";
                VerdictTest = VerdictTestEnum.Ignore;
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            MyVerdict = Verdict.NotSet;
            UpgradeVerdict(MyVerdict);
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            if (Action == SetAction.Command)
            {
                myInstrument.Trigger.SetSequenceDelay((_secondsCPCustomInput ? _secondsCPStr : _secondsCP.ToString()));

            }
            else
            {
                _secondsQR = myInstrument.Trigger.GetSequenceDelay((_presetQPCustomInput ? _presetQPStr : _presetQP.ToString()));

                double? result = _secondsQR;

                if ((result > LowerLimit && result < UpperLimit) && VerdictTest == VerdictTestEnum.InBetween)
                { MyVerdict = Verdict.Pass; }
                else if ((result == ValueEqualTo) && VerdictTest == VerdictTestEnum.EqualTo)
                { MyVerdict = Verdict.Pass; }
                else if ((result >= LowerLimit) && VerdictTest == VerdictTestEnum.GreaterThan)
                { MyVerdict = Verdict.Pass; }
                else if ((result <= UpperLimit) && VerdictTest == VerdictTestEnum.LessThan)
                { MyVerdict = Verdict.Pass; }
                else if (VerdictTest == VerdictTestEnum.Ignore)
                { MyVerdict = Verdict.Pass; }
                else
                { MyVerdict = Verdict.Fail; }
                UpgradeVerdict(MyVerdict);
                if (publishResults)
                {
                    Results.Publish("seconds", new { Seconds = (double)_secondsQR });
                }
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xTriggerSequenceSourceStep
    [Display("SOURce", Groups: new[] { "E364x", "TRIGger", "[SEQuence]" }, Description: "Select the source from which the power supply will accept a trigger.  Query the present trigger source.")]
    public class E364xTriggerSequenceSourceStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x\Docs\e364x_36.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Instrument", Name: "Select Instrument", Order: 10.1)]
        public E364xInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "source Custom Input", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public bool _sourceCPCustomInput { get; set; }
        [Display(Group: "Command Parameter ", Name: "source", Description: "The trigger source.", Order: 30.2)]
        [EnabledIf("_sourceCPCustomInput", false, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public busImmediate _sourceCP { get; set; }
        [Display(Group: "Command Parameter ", Name: "source", Description: "The trigger source.", Order: 30.3)]
        [EnabledIf("_sourceCPCustomInput", true, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public string _sourceCPStr { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "source", Description: "Returns the present trigger source.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public busImmediate _sourceQR { get; private set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xTriggerSequenceSourceStep()
        {
            {
                Name = "TRIGger:[SEQuence]:SOURce";
            }
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            if (Action == SetAction.Command)
            {
                myInstrument.Trigger.SetSequenceSource((_sourceCPCustomInput ? _sourceCPStr : _sourceCP.ToString()));

            }
            else
            {
                _sourceQR = myInstrument.Trigger.GetSequenceSource();
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #endregion
    #endregion
}