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
            if (File.Exists(filePath)) { Log.CreateSource("HelpLink").Debug("Opening HTML file from the location - " + filePath); Process p = notWindows ? Process.Start(new ProcessStartInfo { FileName = "/bin/bash", Arguments = $"-c \"xdg-open '{filePath}'\"", UseShellExecute = false }) : Process.Start(filePath); }
            else { Log.CreateSource("HelpLink").Error("Helplink file could not be found at location - " + filePath); }
        }
    }
    #endregion
    #region Node Classes
    #region APPLy Node Classes
    #region E364xdApplyStep
    [Display("APPLy", Groups: new[] { "Keysight Instrument Basic Control", "E364x Dual Output DC Power Supplies", }, Description: "This command is combination of VOLTage and CURRent commands.  Query the power supply’s present voltage and current setting values and returns a quoted string.")]
    public class E364xdApplyStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x Dual Output DC Power Supplies\Docs\e364x_1.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public E364xdInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "Voltage Custom Input", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public bool _voltageCPCustomInput { get; set; }
        [Display(Group: "Command Parameter ", Name: "Voltage", Description: "Voltage.", Order: 30.2)]
        [EnabledIf("_voltageCPCustomInput", false, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public defMinMax _voltageCP { get; set; }
        [Display(Group: "Command Parameter ", Name: "Voltage", Description: "Voltage.", Order: 30.3)]
        [EnabledIf("_voltageCPCustomInput", true, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public string _voltageCPStr { get; set; }
        [Display(Group: "Command Parameter ", Name: "Current Custom Input", Order: 30.4)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public bool _currentCPCustomInput { get; set; }
        [Display(Group: "Command Parameter ", Name: "Current", Description: "Current.", Order: 30.5)]
        [EnabledIf("_currentCPCustomInput", false, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public defMinMax _currentCP { get; set; }
        [Display(Group: "Command Parameter ", Name: "Current", Description: "Current.", Order: 30.6)]
        [EnabledIf("_currentCPCustomInput", true, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public string _currentCPStr { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "Apply", Description: "Returns the power supply’s present voltage and current setting values and returns a quoted string.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public string _applyQR { get; private set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xdApplyStep()
        {
            Name = ":APPLy";
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
    #region E364xdCalibrationCountStep
    [Display("COUNt", Groups: new[] { "Keysight Instrument Basic Control", "E364x Dual Output DC Power Supplies", "CALibration" }, Description: "Query the power supply to determine the number of times it has been calibrated.")]
    public class E364xdCalibrationCountStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x Dual Output DC Power Supplies\Docs\e364x_59.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public E364xdInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Output]
        [Display(Group: "Query Response ", Name: "Count", Description: "Returns the power supply to determine the number of times it has been calibrated.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public int? _countQR { get; private set; }
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
        public E364xdCalibrationCountStep()
        {
            Name = "CALibration:COUNt";
            VerdictTest = VerdictTestEnum.Ignore;
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            UpgradeVerdict(Verdict.NotSet);
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            _countQR = myInstrument.Calibration.GetCount();

            int? result = _countQR;

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
                Results.Publish("Count", new { Count = (int)_countQR });
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xdCalibrationCurrentDataStep
    [Display("[DATA]", Groups: new[] { "Keysight Instrument Basic Control", "E364x Dual Output DC Power Supplies", "CALibration", "CURRent" }, Description: "This command can only be used after calibration is unsecured and the output state is ON.")]
    public class E364xdCalibrationCurrentDataStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x Dual Output DC Power Supplies\Docs\e364x_60.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public E364xdInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "Numericvalue", Description: "The numeric value.", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public double? _numericValueCP { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xdCalibrationCurrentDataStep()
        {
            Name = "CALibration:CURRent:[DATA]";
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
    #region E364xdCalibrationCurrentLevelStep
    [Display("LEVel", Groups: new[] { "Keysight Instrument Basic Control", "E364x Dual Output DC Power Supplies", "CALibration", "CURRent" }, Description: "This command can only be used after calibration is unsecured and the output state is ON.")]
    public class E364xdCalibrationCurrentLevelStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x Dual Output DC Power Supplies\Docs\e364x_61.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public E364xdInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "Preset Custom Input", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public bool _presetCPCustomInput { get; set; }
        [Display(Group: "Command Parameter ", Name: "Preset", Description: "MINimum | MIDdle | MAXimum", Order: 30.2)]
        [EnabledIf("_presetCPCustomInput", false, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public minMidMax _presetCP { get; set; }
        [Display(Group: "Command Parameter ", Name: "Preset", Description: "MINimum | MIDdle | MAXimum", Order: 30.3)]
        [EnabledIf("_presetCPCustomInput", true, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public string _presetCPStr { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xdCalibrationCurrentLevelStep()
        {
            Name = "CALibration:CURRent:LEVel";
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
    #region E364xdCalibrationSecureCodeStep
    [Display("CODE", Groups: new[] { "Keysight Instrument Basic Control", "E364x Dual Output DC Power Supplies", "CALibration", "SECure" }, Description: "Enter a new security code.")]
    public class E364xdCalibrationSecureCodeStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x Dual Output DC Power Supplies\Docs\e364x_62.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public E364xdInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "Code", Description: "The new security code.", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public string _codeCP { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xdCalibrationSecureCodeStep()
        {
            Name = "CALibration:SECure:CODE";
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
    #region E364xdCalibrationSecureStateStep
    [Display("STATe", Groups: new[] { "Keysight Instrument Basic Control", "E364x Dual Output DC Power Supplies", "CALibration", "SECure" }, Description: "Unsecure or secure the power supply with a security for calibration.  Query the secured state for calibration of the power supply.")]
    public class E364xdCalibrationSecureStateStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x Dual Output DC Power Supplies\Docs\e364x_63.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public E364xdInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "State", Description: "Enable/disable the function.", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public bool _stateCP { get; set; }
        [Display(Group: "Command Parameter ", Name: "Quotedcode", Description: "The quoted code.", Order: 30.2)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public string _quotedCodeCP { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "State", Description: "Returns the current value of the function.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public bool _stateQR { get; private set; }
        #endregion
        #region Verdict Properties
        public enum VerdictTestEnum { EqualTo, Ignore };
        [Display(Group: "Boolean Test", Name: "Select Test", Order: 61.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public VerdictTestEnum VerdictTest { get; set; }

        [Display(Group: "Boolean Test", Name: "Test Value", Order: 61.2)]
        [EnabledIf("VerdictTest", VerdictTestEnum.EqualTo, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool TestValue { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xdCalibrationSecureStateStep()
        {
            Name = "CALibration:SECure:STATe";
            VerdictTest = VerdictTestEnum.Ignore;
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            UpgradeVerdict(Verdict.NotSet);
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
                { UpgradeVerdict(Verdict.Pass); }
                else if (VerdictTest == VerdictTestEnum.Ignore)
                { UpgradeVerdict(Verdict.Pass); }
                else { UpgradeVerdict(Verdict.Fail); }
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xdCalibrationStringStep
    [Display("STRing", Groups: new[] { "Keysight Instrument Basic Control", "E364x Dual Output DC Power Supplies", "CALibration" }, Description: "Record calibration information about your power supply.  Query the calibration message and returns a quoted string.")]
    public class E364xdCalibrationStringStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x Dual Output DC Power Supplies\Docs\e364x_65.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public E364xdInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "Quotedstring", Description: "The calibration information about your power supply.", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public string _quotedStringCP { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "Quotedstring", Description: "Returns the current calibration information about your power supply.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public string _quotedStringQR { get; private set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xdCalibrationStringStep()
        {
            Name = "CALibration:STRing";
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
    #region E364xdCalibrationVoltageDataStep
    [Display("[DATA]", Groups: new[] { "Keysight Instrument Basic Control", "E364x Dual Output DC Power Supplies", "CALibration", "VOLTage" }, Description: "This command can only be used after calibration is unsecured and the output state is ON.")]
    public class E364xdCalibrationVoltageDataStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x Dual Output DC Power Supplies\Docs\e364x_67.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public E364xdInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "Numericvalue", Description: "The numeric value.", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public double? _numericValueCP { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xdCalibrationVoltageDataStep()
        {
            Name = "CALibration:VOLTage:[DATA]";
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
    #region E364xdCalibrationVoltageLevelStep
    [Display("LEVel", Groups: new[] { "Keysight Instrument Basic Control", "E364x Dual Output DC Power Supplies", "CALibration", "VOLTage" }, Description: "This command can only be used after calibration is unsecured and the output state is ON.")]
    public class E364xdCalibrationVoltageLevelStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x Dual Output DC Power Supplies\Docs\e364x_68.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public E364xdInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "Preset Custom Input", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public bool _presetCPCustomInput { get; set; }
        [Display(Group: "Command Parameter ", Name: "Preset", Description: "MINimum | MIDdle | MAXimum", Order: 30.2)]
        [EnabledIf("_presetCPCustomInput", false, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public minMidMax _presetCP { get; set; }
        [Display(Group: "Command Parameter ", Name: "Preset", Description: "MINimum | MIDdle | MAXimum", Order: 30.3)]
        [EnabledIf("_presetCPCustomInput", true, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public string _presetCPStr { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xdCalibrationVoltageLevelStep()
        {
            Name = "CALibration:VOLTage:LEVel";
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
    #region E364xdCalibrationVoltageProtectionStep
    [Display("PROTection", Groups: new[] { "Keysight Instrument Basic Control", "E364x Dual Output DC Power Supplies", "CALibration", "VOLTage" }, Description: "Calibrate the overvoltage protection circuit of the power supply.")]
    public class E364xdCalibrationVoltageProtectionStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x Dual Output DC Power Supplies\Docs\e364x_69.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public E364xdInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xdCalibrationVoltageProtectionStep()
        {
            Name = "CALibration:VOLTage:PROTection";
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
    #region E364xdDisplayWindowModeStep
    [Display("MODE", Groups: new[] { "Keysight Instrument Basic Control", "E364x Dual Output DC Power Supplies", "DISPlay", "[WINDow]" }, Description: "Set the front-panel display mode of the power supply.  Query the state of the display mode.")]
    public class E364xdDisplayWindowModeStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x Dual Output DC Power Supplies\Docs\e364x_40.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public E364xdInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "Mode Custom Input", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public bool _modeCPCustomInput { get; set; }
        [Display(Group: "Command Parameter ", Name: "Mode", Description: "The front-panel display mode of the power supply.", Order: 30.2)]
        [EnabledIf("_modeCPCustomInput", false, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public mode _modeCP { get; set; }
        [Display(Group: "Command Parameter ", Name: "Mode", Description: "The front-panel display mode of the power supply.", Order: 30.3)]
        [EnabledIf("_modeCPCustomInput", true, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public string _modeCPStr { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "Mode", Description: "Returns the current front-panel display mode of the power supply.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public mode _modeQR { get; private set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xdDisplayWindowModeStep()
        {
            Name = "DISPlay:[WINDow]:MODE";
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
    #region E364xdDisplayWindowStateStep
    [Display("[STATe]", Groups: new[] { "Keysight Instrument Basic Control", "E364x Dual Output DC Power Supplies", "DISPlay", "[WINDow]" }, Description: "Turn the front-panel display off or on.   Query the front-panel display setting.")]
    public class E364xdDisplayWindowStateStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x Dual Output DC Power Supplies\Docs\e364x_39.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public E364xdInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "State", Description: "Enable/disable the front-panel display.", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public bool _stateCP { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "State", Description: "Returns the current value of the front-panel display.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public bool _stateQR { get; private set; }
        #endregion
        #region Verdict Properties
        public enum VerdictTestEnum { EqualTo, Ignore };
        [Display(Group: "Boolean Test", Name: "Select Test", Order: 61.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public VerdictTestEnum VerdictTest { get; set; }

        [Display(Group: "Boolean Test", Name: "Test Value", Order: 61.2)]
        [EnabledIf("VerdictTest", VerdictTestEnum.EqualTo, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool TestValue { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xdDisplayWindowStateStep()
        {
            Name = "DISPlay:[WINDow]:[STATe]";
            VerdictTest = VerdictTestEnum.Ignore;
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            UpgradeVerdict(Verdict.NotSet);
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
                { UpgradeVerdict(Verdict.Pass); }
                else if (VerdictTest == VerdictTestEnum.Ignore)
                { UpgradeVerdict(Verdict.Pass); }
                else { UpgradeVerdict(Verdict.Fail); }
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xdDisplayWindowTextClearStep
    [Display("CLEar", Groups: new[] { "Keysight Instrument Basic Control", "E364x Dual Output DC Power Supplies", "DISPlay", "[WINDow]", "TEXT" }, Description: "Clear the message displayed on the front panel.")]
    public class E364xdDisplayWindowTextClearStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x Dual Output DC Power Supplies\Docs\e364x_45.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public E364xdInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xdDisplayWindowTextClearStep()
        {
            Name = "DISPlay:[WINDow]:TEXT:CLEar";
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
    #region E364xdDisplayWindowTextDataStep
    [Display("[DATA]", Groups: new[] { "Keysight Instrument Basic Control", "E364x Dual Output DC Power Supplies", "DISPlay", "[WINDow]", "TEXT" }, Description: "Display a message on the front panel.   Query the message sent to the front panel and returns a quoted string.")]
    public class E364xdDisplayWindowTextDataStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x Dual Output DC Power Supplies\Docs\e364x_43.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public E364xdInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "Quotedstring", Description: "The front panel message.", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public string _quotedStringCP { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "Quotedstring", Description: "Returns the front panel message.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public string _quotedStringQR { get; private set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xdDisplayWindowTextDataStep()
        {
            Name = "DISPlay:[WINDow]:TEXT:[DATA]";
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
    #region E364xdInitiateImmediateStep
    [Display("[IMMediate]", Groups: new[] { "Keysight Instrument Basic Control", "E364x Dual Output DC Power Supplies", "INITiate" }, Description: "Cause the trigger system to initiate.")]
    public class E364xdInitiateImmediateStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x Dual Output DC Power Supplies\Docs\e364x_33.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public E364xdInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xdInitiateImmediateStep()
        {
            Name = "INITiate:[IMMediate]";
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
    #region E364xdInstrumentSelectStep
    [Display("[SELect]", Groups: new[] { "Keysight Instrument Basic Control", "E364x Dual Output DC Power Supplies", "INSTrument" }, Description: "Select the output to be programmed one of the two outputs by the output identifier.  Return the currently selected output by the INSTrument{:SELect] or INSTrument:NSELect command.")]
    public class E364xdInstrumentSelectStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x Dual Output DC Power Supplies\Docs\e364x_9.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public E364xdInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "Select Custom Input", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public bool _selectCPCustomInput { get; set; }
        [Display(Group: "Command Parameter ", Name: "Select", Description: "The output identifier.", Order: 30.2)]
        [EnabledIf("_selectCPCustomInput", false, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public channel _selectCP { get; set; }
        [Display(Group: "Command Parameter ", Name: "Select", Description: "The output identifier.", Order: 30.3)]
        [EnabledIf("_selectCPCustomInput", true, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public string _selectCPStr { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "Select", Description: "Returns the current output identifier.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public channel _selectQR { get; private set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xdInstrumentSelectStep()
        {
            Name = "INSTrument:[SELect]";
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
    #region E364xdInstrumentNselectStep
    [Display("NSELect", Groups: new[] { "Keysight Instrument Basic Control", "E364x Dual Output DC Power Supplies", "INSTrument" }, Description: "Select the output to be programmed one of the two outputs by a numeric value instead of the output identifier used in the INSTrument:NSELect or INSTrument[:SELect] command.  Return the currently selected output by the INSTrument[SELect]or INSTrument[SELect] command.")]
    public class E364xdInstrumentNselectStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x Dual Output DC Power Supplies\Docs\e364x_11.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public E364xdInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "Nselect", Description: "Instrument output.", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public int? _nselectCP { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "Nselect", Description: "Return the currently selected output by the INSTrument[SELect]", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public int? _nselectQR { get; private set; }
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
        public E364xdInstrumentNselectStep()
        {
            Name = "INSTrument:NSELect";
            VerdictTest = VerdictTestEnum.Ignore;
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            UpgradeVerdict(Verdict.NotSet);
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
                    Results.Publish("Nselect", new { Nselect = (int)_nselectQR });
                }
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xdInstrumentCoupleTriggerStep
    [Display("[TRIGger]", Groups: new[] { "Keysight Instrument Basic Control", "E364x Dual Output DC Power Supplies", "INSTrument", "COUPle" }, Description: "Enable or disable a coupling between two logical outputs of the power supply.  Query the output coupling state of the power supply.")]
    public class E364xdInstrumentCoupleTriggerStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x Dual Output DC Power Supplies\Docs\e364x_13.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public E364xdInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "State", Description: "Enable/disable the power supply function.", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public bool _stateCP { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "State", Description: "Returns the current value of the power supply function.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public int? _stateQR { get; private set; }
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
        public E364xdInstrumentCoupleTriggerStep()
        {
            Name = "INSTrument:COUPle:[TRIGger]";
            VerdictTest = VerdictTestEnum.Ignore;
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            UpgradeVerdict(Verdict.NotSet);
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
                    Results.Publish("State", new { State = (int)_stateQR });
                }
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #endregion
    #region MEASure Node Classes
    #region E364xdMeasureScalarCurrentDcStep
    [Display("[DC]", Groups: new[] { "Keysight Instrument Basic Control", "E364x Dual Output DC Power Supplies", "MEASure", "[SCALar]", "CURRent" }, Description: "Query the current measured across the current sense resistor inside the power supply.")]
    public class E364xdMeasureScalarCurrentDcStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x Dual Output DC Power Supplies\Docs\e364x_15.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public E364xdInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Output]
        [Display(Group: "Query Response ", Name: "Current", Description: "Returns the current measured across the current sense resistor inside the power supply.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public double? _currentQR { get; private set; }
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
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xdMeasureScalarCurrentDcStep()
        {
            Name = "MEASure:[SCALar]:CURRent:[DC]";
            VerdictTest = VerdictTestEnum.Ignore;
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            UpgradeVerdict(Verdict.NotSet);
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            _currentQR = myInstrument.Measure.GetScalarCurrentDc();

            double? result = _currentQR;

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
                Results.Publish("Current", new { Current = (double)_currentQR });
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xdMeasureScalarVoltageDcStep
    [Display("[DC]", Groups: new[] { "Keysight Instrument Basic Control", "E364x Dual Output DC Power Supplies", "MEASure", "[SCALar]", "[VOLTage]" }, Description: "Query the voltage measured at the sense terminals of the power supply.")]
    public class E364xdMeasureScalarVoltageDcStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x Dual Output DC Power Supplies\Docs\e364x_16.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public E364xdInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Output]
        [Display(Group: "Query Response ", Name: "Voltage", Description: "Returns the voltage measured at the sense terminals of the power supply.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public double? _voltageQR { get; private set; }
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
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xdMeasureScalarVoltageDcStep()
        {
            Name = "MEASure:[SCALar]:[VOLTage]:[DC]";
            VerdictTest = VerdictTestEnum.Ignore;
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            UpgradeVerdict(Verdict.NotSet);
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            _voltageQR = myInstrument.Measure.GetScalarVoltageDc();

            double? result = _voltageQR;

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
                Results.Publish("Voltage", new { Voltage = (double)_voltageQR });
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #endregion
    #region MEMory Node Classes
    #region E364xdMemoryStateNameStep
    [Display("NAME", Groups: new[] { "Keysight Instrument Basic Control", "E364x Dual Output DC Power Supplies", "MEMory", "STATe" }, Description: "Assign a name to the specified storage location.")]
    public class E364xdMemoryStateNameStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x Dual Output DC Power Supplies\Docs\e364x_58.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public E364xdInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "Name", Description: "The storage location.", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public int? _nameCP { get; set; }
        [Display(Group: "Command Parameter ", Name: "Quotedname", Description: "Name of the storage location.", Order: 30.2)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public string _quotedNameCP { get; set; }
        [Display(Group: "Query Parameter ", Name: "Name", Description: "The storage location.", Order: 40.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public int? _nameQP { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "Quotedname", Description: "Returns the current value of name of the storage location.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public string _quotedNameQR { get; private set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xdMemoryStateNameStep()
        {
            Name = "MEMory:STATe:NAME";
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
    #region E364xdOutputStateStep
    [Display("[STATe]", Groups: new[] { "Keysight Instrument Basic Control", "E364x Dual Output DC Power Supplies", "OUTPut" }, Description: "Enable or disable the outputs of the power supply.   Query the output state of the power supply.")]
    public class E364xdOutputStateStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x Dual Output DC Power Supplies\Docs\e364x_46.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public E364xdInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "State", Description: "Enable/disable the outputs of the power supply.", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public bool _stateCP { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "State", Description: "Returns the current value of the outputs of the power supply.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public bool _stateQR { get; private set; }
        #endregion
        #region Verdict Properties
        public enum VerdictTestEnum { EqualTo, Ignore };
        [Display(Group: "Boolean Test", Name: "Select Test", Order: 61.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public VerdictTestEnum VerdictTest { get; set; }

        [Display(Group: "Boolean Test", Name: "Test Value", Order: 61.2)]
        [EnabledIf("VerdictTest", VerdictTestEnum.EqualTo, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool TestValue { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xdOutputStateStep()
        {
            Name = "OUTPut:[STATe]";
            VerdictTest = VerdictTestEnum.Ignore;
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            UpgradeVerdict(Verdict.NotSet);
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
                { UpgradeVerdict(Verdict.Pass); }
                else if (VerdictTest == VerdictTestEnum.Ignore)
                { UpgradeVerdict(Verdict.Pass); }
                else { UpgradeVerdict(Verdict.Fail); }
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xdOutputRelayStateStep
    [Display("[STATe]", Groups: new[] { "Keysight Instrument Basic Control", "E364x Dual Output DC Power Supplies", "OUTPut", "RELay" }, Description: "Set the state of two TTL signals on the RS-232 connector pin 1 and pin 9. These signals are intended for use with an external relay and relay driver. At *RST, the OUTPUT:RELay state is OFF.    Query the state of the TTL relay logic signals.")]
    public class E364xdOutputRelayStateStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x Dual Output DC Power Supplies\Docs\e364x_48.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public E364xdInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "State", Description: "State of the function.", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public bool _stateCP { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "State", Description: "Returns the current value of state of the function.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public bool _stateQR { get; private set; }
        #endregion
        #region Verdict Properties
        public enum VerdictTestEnum { EqualTo, Ignore };
        [Display(Group: "Boolean Test", Name: "Select Test", Order: 61.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public VerdictTestEnum VerdictTest { get; set; }

        [Display(Group: "Boolean Test", Name: "Test Value", Order: 61.2)]
        [EnabledIf("VerdictTest", VerdictTestEnum.EqualTo, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool TestValue { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xdOutputRelayStateStep()
        {
            Name = "OUTPut:RELay:[STATe]";
            VerdictTest = VerdictTestEnum.Ignore;
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            UpgradeVerdict(Verdict.NotSet);
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
                { UpgradeVerdict(Verdict.Pass); }
                else if (VerdictTest == VerdictTestEnum.Ignore)
                { UpgradeVerdict(Verdict.Pass); }
                else { UpgradeVerdict(Verdict.Fail); }
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xdOutputTrackStateStep
    [Display("[STATe]", Groups: new[] { "Keysight Instrument Basic Control", "E364x Dual Output DC Power Supplies", "OUTPut", "TRACk" }, Description: "Enable or disable the power supply to operate in the track mode.   Query the tracking mode state of the power supply.")]
    public class E364xdOutputTrackStateStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x Dual Output DC Power Supplies\Docs\e364x_17.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public E364xdInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "State", Description: "Enable/disable the power supply to operate in the track mode.", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public bool _stateCP { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "State", Description: "Returns the current value of the power supply to operate in the track mode.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public bool _stateQR { get; private set; }
        #endregion
        #region Verdict Properties
        public enum VerdictTestEnum { EqualTo, Ignore };
        [Display(Group: "Boolean Test", Name: "Select Test", Order: 61.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public VerdictTestEnum VerdictTest { get; set; }

        [Display(Group: "Boolean Test", Name: "Test Value", Order: 61.2)]
        [EnabledIf("VerdictTest", VerdictTestEnum.EqualTo, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool TestValue { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xdOutputTrackStateStep()
        {
            Name = "OUTPut:TRACk:[STATe]";
            VerdictTest = VerdictTestEnum.Ignore;
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            UpgradeVerdict(Verdict.NotSet);
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
                { UpgradeVerdict(Verdict.Pass); }
                else if (VerdictTest == VerdictTestEnum.Ignore)
                { UpgradeVerdict(Verdict.Pass); }
                else { UpgradeVerdict(Verdict.Fail); }
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #endregion
    #region SOURce Node Classes
    #region E364xdSourceCurrentLevelImmediateAmplitudeStep
    [Display("[AMPLitude]", Groups: new[] { "Keysight Instrument Basic Control", "E364x Dual Output DC Power Supplies", "[SOURce]", "CURRent", "[LEVel]", "[IMMediate]" }, Description: "Program the immediate current level of the power supply.   Return the presently programmed current level of the power supply.")]
    public class E364xdSourceCurrentLevelImmediateAmplitudeStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x Dual Output DC Power Supplies\Docs\e364x_3.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public E364xdInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "Current Custom Input", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public bool _currentCPCustomInput { get; set; }
        [Display(Group: "Command Parameter ", Name: "Current", Description: "The immediate current level of the power supply.", Order: 30.2)]
        [EnabledIf("_currentCPCustomInput", false, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public minMaxUpDown _currentCP { get; set; }
        [Display(Group: "Command Parameter ", Name: "Current", Description: "The immediate current level of the power supply.", Order: 30.3)]
        [EnabledIf("_currentCPCustomInput", true, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public string _currentCPStr { get; set; }
        [Display(Group: "Query Parameter ", Name: "Preset Custom Input", Order: 40.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public bool _presetQPCustomInput { get; set; }
        [Display(Group: "Query Parameter ", Name: "Preset", Description: "MINimum | MAXimum | UP | DOWN", Order: 40.2)]
        [EnabledIf("_presetQPCustomInput", false, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public minMax _presetQP { get; set; }
        [Display(Group: "Query Parameter ", Name: "Preset", Description: "MINimum | MAXimum | UP | DOWN", Order: 40.3)]
        [EnabledIf("_presetQPCustomInput", true, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public string _presetQPStr { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "Current", Description: "Returns the current value of the immediate current level of the power supply.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public double? _currentQR { get; private set; }
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
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xdSourceCurrentLevelImmediateAmplitudeStep()
        {
            Name = "[SOURce]:CURRent:[LEVel]:[IMMediate]:[AMPLitude]";
            VerdictTest = VerdictTestEnum.Ignore;
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            UpgradeVerdict(Verdict.NotSet);
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
                    Results.Publish("Current", new { Current = (double)_currentQR });
                }
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xdSourceCurrentLevelImmediateStepIncrementStep
    [Display("[INCRement]", Groups: new[] { "Keysight Instrument Basic Control", "E364x Dual Output DC Power Supplies", "[SOURce]", "CURRent", "[LEVel]", "[IMMediate]", "STEP" }, Description: "Set the step size for current programming with the CURRent UPand CURRentDOWN commands.   Return the value of the step size currently specified.")]
    public class E364xdSourceCurrentLevelImmediateStepIncrementStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x Dual Output DC Power Supplies\Docs\e364x_5.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public E364xdInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "Numericvalue Custom Input", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public bool _numericValueCPCustomInput { get; set; }
        [Display(Group: "Command Parameter ", Name: "Numericvalue", Description: "The step size for current programming with the CURRent UPand CURRentDOWN commands.", Order: 30.2)]
        [EnabledIf("_numericValueCPCustomInput", false, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public _default _numericValueCP { get; set; }
        [Display(Group: "Command Parameter ", Name: "Numericvalue", Description: "The step size for current programming with the CURRent UPand CURRentDOWN commands.", Order: 30.3)]
        [EnabledIf("_numericValueCPCustomInput", true, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public string _numericValueCPStr { get; set; }
        [Display(Group: "Query Parameter ", Name: "Default Custom Input", Order: 40.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public bool _defaultQPCustomInput { get; set; }
        [Display(Group: "Query Parameter ", Name: "Default", Description: "Default.", Order: 40.2)]
        [EnabledIf("_defaultQPCustomInput", false, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public _default _defaultQP { get; set; }
        [Display(Group: "Query Parameter ", Name: "Default", Description: "Default.", Order: 40.3)]
        [EnabledIf("_defaultQPCustomInput", true, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public string _defaultQPStr { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "Numericvalue", Description: "Return the value of the step size currently specified.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public double? _numericValueQR { get; private set; }
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
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xdSourceCurrentLevelImmediateStepIncrementStep()
        {
            Name = "[SOURce]:CURRent:[LEVel]:[IMMediate]:STEP:[INCRement]";
            VerdictTest = VerdictTestEnum.Ignore;
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            UpgradeVerdict(Verdict.NotSet);
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
                    Results.Publish("Numericvalue", new { Numericvalue = (double)_numericValueQR });
                }
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xdSourceCurrentLevelTriggeredAmplitudeStep
    [Display("[AMPLitude]", Groups: new[] { "Keysight Instrument Basic Control", "E364x Dual Output DC Power Supplies", "[SOURce]", "CURRent", "[LEVel]", "TRIGgered" }, Description: "Program the pending triggered current level.   Query the triggered current level presently programmed.")]
    public class E364xdSourceCurrentLevelTriggeredAmplitudeStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x Dual Output DC Power Supplies\Docs\e364x_7.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public E364xdInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "Current Custom Input", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public bool _currentCPCustomInput { get; set; }
        [Display(Group: "Command Parameter ", Name: "Current", Description: "The the pending triggered current level.", Order: 30.2)]
        [EnabledIf("_currentCPCustomInput", false, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public minMax _currentCP { get; set; }
        [Display(Group: "Command Parameter ", Name: "Current", Description: "The the pending triggered current level.", Order: 30.3)]
        [EnabledIf("_currentCPCustomInput", true, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public string _currentCPStr { get; set; }
        [Display(Group: "Query Parameter ", Name: "Preset Custom Input", Order: 40.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public bool _presetQPCustomInput { get; set; }
        [Display(Group: "Query Parameter ", Name: "Preset", Description: "MINimum | MAXimum", Order: 40.2)]
        [EnabledIf("_presetQPCustomInput", false, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public minMax _presetQP { get; set; }
        [Display(Group: "Query Parameter ", Name: "Preset", Description: "MINimum | MAXimum", Order: 40.3)]
        [EnabledIf("_presetQPCustomInput", true, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public string _presetQPStr { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "Current", Description: "Returns the triggered current level presently programmed.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public double? _currentQR { get; private set; }
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
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xdSourceCurrentLevelTriggeredAmplitudeStep()
        {
            Name = "[SOURce]:CURRent:[LEVel]:TRIGgered:[AMPLitude]";
            VerdictTest = VerdictTestEnum.Ignore;
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            UpgradeVerdict(Verdict.NotSet);
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
                    Results.Publish("Current", new { Current = (double)_currentQR });
                }
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xdSourceVoltageRangeStep
    [Display("RANGe", Groups: new[] { "Keysight Instrument Basic Control", "E364x Dual Output DC Power Supplies", "[SOURce]", "VOLTage" }, Description: "Select an output range to be programmed by the identifier.  Query the currently selected range.")]
    public class E364xdSourceVoltageRangeStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x Dual Output DC Power Supplies\Docs\e364x_31.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public E364xdInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "Range Custom Input", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public bool _rangeCPCustomInput { get; set; }
        [Display(Group: "Command Parameter ", Name: "Range", Description: "The range.", Order: 30.2)]
        [EnabledIf("_rangeCPCustomInput", false, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public output _rangeCP { get; set; }
        [Display(Group: "Command Parameter ", Name: "Range", Description: "The range.", Order: 30.3)]
        [EnabledIf("_rangeCPCustomInput", true, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public string _rangeCPStr { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "Range", Description: "Returns the currently selected range.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public output _rangeQR { get; private set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xdSourceVoltageRangeStep()
        {
            Name = "[SOURce]:VOLTage:RANGe";
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
    #region E364xdSourceVoltageProtectionClearStep
    [Display("CLEar", Groups: new[] { "Keysight Instrument Basic Control", "E364x Dual Output DC Power Supplies", "[SOURce]", "VOLTage", "PROTection" }, Description: "Cause the overvoltage protection circuit to be cleared.")]
    public class E364xdSourceVoltageProtectionClearStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x Dual Output DC Power Supplies\Docs\e364x_30.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public E364xdInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xdSourceVoltageProtectionClearStep()
        {
            Name = "[SOURce]:VOLTage:PROTection:CLEar";
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
    #region E364xdSourceVoltageProtectionLevelStep
    [Display("[LEVel]", Groups: new[] { "Keysight Instrument Basic Control", "E364x Dual Output DC Power Supplies", "[SOURce]", "VOLTage", "PROTection" }, Description: "Set the voltage level at which the overvoltage protection (OVP) circuit will trip.   Query the overvoltage protection trip level presently programmed.")]
    public class E364xdSourceVoltageProtectionLevelStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x Dual Output DC Power Supplies\Docs\e364x_25.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public E364xdInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "Voltage Custom Input", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public bool _voltageCPCustomInput { get; set; }
        [Display(Group: "Command Parameter ", Name: "Voltage", Description: "The voltage level at which the overvoltage protection (OVP) circuit will trip.", Order: 30.2)]
        [EnabledIf("_voltageCPCustomInput", false, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public minMax _voltageCP { get; set; }
        [Display(Group: "Command Parameter ", Name: "Voltage", Description: "The voltage level at which the overvoltage protection (OVP) circuit will trip.", Order: 30.3)]
        [EnabledIf("_voltageCPCustomInput", true, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public string _voltageCPStr { get; set; }
        [Display(Group: "Query Parameter ", Name: "Preset Custom Input", Order: 40.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public bool _presetQPCustomInput { get; set; }
        [Display(Group: "Query Parameter ", Name: "Preset", Description: "MINimum|MAXimum", Order: 40.2)]
        [EnabledIf("_presetQPCustomInput", false, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public minMax _presetQP { get; set; }
        [Display(Group: "Query Parameter ", Name: "Preset", Description: "MINimum|MAXimum", Order: 40.3)]
        [EnabledIf("_presetQPCustomInput", true, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public string _presetQPStr { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "Voltage", Description: "Returns the current value of the voltage level at which the overvoltage protection (OVP) circuit will trip.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public double? _voltageQR { get; private set; }
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
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xdSourceVoltageProtectionLevelStep()
        {
            Name = "[SOURce]:VOLTage:PROTection:[LEVel]";
            VerdictTest = VerdictTestEnum.Ignore;
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            UpgradeVerdict(Verdict.NotSet);
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
                    Results.Publish("Voltage", new { Voltage = (double)_voltageQR });
                }
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xdSourceVoltageProtectionStateStep
    [Display("STATe", Groups: new[] { "Keysight Instrument Basic Control", "E364x Dual Output DC Power Supplies", "[SOURce]", "VOLTage", "PROTection" }, Description: "Enable or disable the overvoltage protection function.  Query the state of the overvoltage protection function.")]
    public class E364xdSourceVoltageProtectionStateStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x Dual Output DC Power Supplies\Docs\e364x_27.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public E364xdInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "State", Description: "Enable/disable the overvoltage protection function.", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public bool _stateCP { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "State", Description: "Returns the current value of the overvoltage protection function.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public bool _stateQR { get; private set; }
        #endregion
        #region Verdict Properties
        public enum VerdictTestEnum { EqualTo, Ignore };
        [Display(Group: "Boolean Test", Name: "Select Test", Order: 61.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public VerdictTestEnum VerdictTest { get; set; }

        [Display(Group: "Boolean Test", Name: "Test Value", Order: 61.2)]
        [EnabledIf("VerdictTest", VerdictTestEnum.EqualTo, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool TestValue { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xdSourceVoltageProtectionStateStep()
        {
            Name = "[SOURce]:VOLTage:PROTection:STATe";
            VerdictTest = VerdictTestEnum.Ignore;
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            UpgradeVerdict(Verdict.NotSet);
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
                { UpgradeVerdict(Verdict.Pass); }
                else if (VerdictTest == VerdictTestEnum.Ignore)
                { UpgradeVerdict(Verdict.Pass); }
                else { UpgradeVerdict(Verdict.Fail); }
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xdSourceVoltageProtectionTrippedStep
    [Display("TRIPped", Groups: new[] { "Keysight Instrument Basic Control", "E364x Dual Output DC Power Supplies", "[SOURce]", "VOLTage", "PROTection" }, Description: "Return a ‘‘1’’ if the overvoltage protection circuit is tripped and not cleared or a ‘‘0’’ if not tripped.")]
    public class E364xdSourceVoltageProtectionTrippedStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x Dual Output DC Power Supplies\Docs\e364x_29.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public E364xdInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Output]
        [Display(Group: "Query Response ", Name: "Tripped", Description: "Returns the current value of the function.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public bool _trippedQR { get; private set; }
        #endregion
        #region Verdict Properties
        public enum VerdictTestEnum { EqualTo, Ignore };
        [Display(Group: "Boolean Test", Name: "Select Test", Order: 61.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public VerdictTestEnum VerdictTest { get; set; }

        [Display(Group: "Boolean Test", Name: "Test Value", Order: 61.2)]
        [EnabledIf("VerdictTest", VerdictTestEnum.EqualTo, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]
        public bool TestValue { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xdSourceVoltageProtectionTrippedStep()
        {
            Name = "[SOURce]:VOLTage:PROTection:TRIPped";
            VerdictTest = VerdictTestEnum.Ignore;
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            UpgradeVerdict(Verdict.NotSet);
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            _trippedQR = myInstrument.Source.GetVoltageProtectionTripped();

            bool result = _trippedQR;

            if (result == TestValue && VerdictTest == VerdictTestEnum.EqualTo)
            { UpgradeVerdict(Verdict.Pass); }
            else if (VerdictTest == VerdictTestEnum.Ignore)
            { UpgradeVerdict(Verdict.Pass); }
            else { UpgradeVerdict(Verdict.Fail); }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xdSourceVoltageLevelImmediateAmplitudeStep
    [Display("[AMPLitude]", Groups: new[] { "Keysight Instrument Basic Control", "E364x Dual Output DC Power Supplies", "[SOURce]", "VOLTage", "[LEVel]", "[IMMediate]" }, Description: "Program the immediate voltage level of the power supply.   Query the presently programmed voltage level of the power supply.")]
    public class E364xdSourceVoltageLevelImmediateAmplitudeStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x Dual Output DC Power Supplies\Docs\e364x_19.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public E364xdInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "Voltage Custom Input", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public bool _voltageCPCustomInput { get; set; }
        [Display(Group: "Command Parameter ", Name: "Voltage", Description: "The immediate voltage level of the power supply.", Order: 30.2)]
        [EnabledIf("_voltageCPCustomInput", false, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public minMaxUpDown _voltageCP { get; set; }
        [Display(Group: "Command Parameter ", Name: "Voltage", Description: "The immediate voltage level of the power supply.", Order: 30.3)]
        [EnabledIf("_voltageCPCustomInput", true, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public string _voltageCPStr { get; set; }
        [Display(Group: "Query Parameter ", Name: "Preset Custom Input", Order: 40.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public bool _presetQPCustomInput { get; set; }
        [Display(Group: "Query Parameter ", Name: "Preset", Description: "MINimum | MAXimum | UP | DOWN", Order: 40.2)]
        [EnabledIf("_presetQPCustomInput", false, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public minMax _presetQP { get; set; }
        [Display(Group: "Query Parameter ", Name: "Preset", Description: "MINimum | MAXimum | UP | DOWN", Order: 40.3)]
        [EnabledIf("_presetQPCustomInput", true, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public string _presetQPStr { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "Voltage", Description: "Returnsthe presently programmed voltage level of the power supply.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public double? _voltageQR { get; private set; }
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
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xdSourceVoltageLevelImmediateAmplitudeStep()
        {
            Name = "[SOURce]:VOLTage:[LEVel]:[IMMediate]:[AMPLitude]";
            VerdictTest = VerdictTestEnum.Ignore;
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            UpgradeVerdict(Verdict.NotSet);
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
                    Results.Publish("Voltage", new { Voltage = (double)_voltageQR });
                }
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xdSourceVoltageLevelImmediateStepIncrementStep
    [Display("[INCRement]", Groups: new[] { "Keysight Instrument Basic Control", "E364x Dual Output DC Power Supplies", "[SOURce]", "VOLTage", "[LEVel]", "[IMMediate]", "STEP" }, Description: "Set the step size for voltage programming with the VOLT UP and VOLT DOWN commands.  Return the value of the step size currently specified.")]
    public class E364xdSourceVoltageLevelImmediateStepIncrementStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x Dual Output DC Power Supplies\Docs\e364x_21.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public E364xdInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "Numericvalue Custom Input", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public bool _numericValueCPCustomInput { get; set; }
        [Display(Group: "Command Parameter ", Name: "Numericvalue", Description: "The value of the step size currently specified.", Order: 30.2)]
        [EnabledIf("_numericValueCPCustomInput", false, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public _default _numericValueCP { get; set; }
        [Display(Group: "Command Parameter ", Name: "Numericvalue", Description: "The value of the step size currently specified.", Order: 30.3)]
        [EnabledIf("_numericValueCPCustomInput", true, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public string _numericValueCPStr { get; set; }
        [Display(Group: "Query Parameter ", Name: "Default Custom Input", Order: 40.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public bool _defaultQPCustomInput { get; set; }
        [Display(Group: "Query Parameter ", Name: "Default", Description: "Default.", Order: 40.2)]
        [EnabledIf("_defaultQPCustomInput", false, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public _default _defaultQP { get; set; }
        [Display(Group: "Query Parameter ", Name: "Default", Description: "Default.", Order: 40.3)]
        [EnabledIf("_defaultQPCustomInput", true, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public string _defaultQPStr { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "Numericvalue", Description: "Returns the current  value of the step size currently specified.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public double? _numericValueQR { get; private set; }
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
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xdSourceVoltageLevelImmediateStepIncrementStep()
        {
            Name = "[SOURce]:VOLTage:[LEVel]:[IMMediate]:STEP:[INCRement]";
            VerdictTest = VerdictTestEnum.Ignore;
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            UpgradeVerdict(Verdict.NotSet);
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
                    Results.Publish("Numericvalue", new { Numericvalue = (double)_numericValueQR });
                }
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xdSourceVoltageLevelTriggeredAmplitudeStep
    [Display("[AMPLitude]", Groups: new[] { "Keysight Instrument Basic Control", "E364x Dual Output DC Power Supplies", "[SOURce]", "VOLTage", "[LEVel]", "TRIGgered" }, Description: "Program the pending triggered voltage level.   Query the triggered voltage level presently programmed.")]
    public class E364xdSourceVoltageLevelTriggeredAmplitudeStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x Dual Output DC Power Supplies\Docs\e364x_23.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public E364xdInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "Voltage Custom Input", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public bool _voltageCPCustomInput { get; set; }
        [Display(Group: "Command Parameter ", Name: "Voltage", Description: "The pending triggered voltage level.", Order: 30.2)]
        [EnabledIf("_voltageCPCustomInput", false, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public minMax _voltageCP { get; set; }
        [Display(Group: "Command Parameter ", Name: "Voltage", Description: "The pending triggered voltage level.", Order: 30.3)]
        [EnabledIf("_voltageCPCustomInput", true, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public string _voltageCPStr { get; set; }
        [Display(Group: "Query Parameter ", Name: "Preset Custom Input", Order: 40.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public bool _presetQPCustomInput { get; set; }
        [Display(Group: "Query Parameter ", Name: "Preset", Description: "MINimum | MAXimum", Order: 40.2)]
        [EnabledIf("_presetQPCustomInput", false, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public minMax _presetQP { get; set; }
        [Display(Group: "Query Parameter ", Name: "Preset", Description: "MINimum | MAXimum", Order: 40.3)]
        [EnabledIf("_presetQPCustomInput", true, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public string _presetQPStr { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "Voltage", Description: "Returns the triggered voltage level presently programmed.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public double? _voltageQR { get; private set; }
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
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xdSourceVoltageLevelTriggeredAmplitudeStep()
        {
            Name = "[SOURce]:VOLTage:[LEVel]:TRIGgered:[AMPLitude]";
            VerdictTest = VerdictTestEnum.Ignore;
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            UpgradeVerdict(Verdict.NotSet);
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
                    Results.Publish("Voltage", new { Voltage = (double)_voltageQR });
                }
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #endregion
    #region STATus Node Classes
    #region E364xdStatusQuestionableEnableStep
    [Display("ENABle", Groups: new[] { "Keysight Instrument Basic Control", "E364x Dual Output DC Power Supplies", "STATus", "QUEStionable" }, Description: "Enable bits in the Questionable Status Enable register.   Query the Questionable Status Enable register.")]
    public class E364xdStatusQuestionableEnableStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x Dual Output DC Power Supplies\Docs\e364x_75.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public E364xdInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "Enable", Description: "The Questionable Status Enable register.", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public int? _enableCP { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "Enable", Description: "Returns the Questionable Status Enable register.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public int? _enableQR { get; private set; }
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
        public E364xdStatusQuestionableEnableStep()
        {
            Name = "STATus:QUEStionable:ENABle";
            VerdictTest = VerdictTestEnum.Ignore;
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            UpgradeVerdict(Verdict.NotSet);
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
                    Results.Publish("Enable", new { Enable = (int)_enableQR });
                }
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xdStatusQuestionableEventStep
    [Display("[EVENt]", Groups: new[] { "Keysight Instrument Basic Control", "E364x Dual Output DC Power Supplies", "STATus", "QUEStionable" }, Description: "Query the Questionable Status Event register.")]
    public class E364xdStatusQuestionableEventStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x Dual Output DC Power Supplies\Docs\e364x_74.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public E364xdInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Output]
        [Display(Group: "Query Response ", Name: "Event", Description: "Returns the Questionable Status Event register.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public int? _eventQR { get; private set; }
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
        public E364xdStatusQuestionableEventStep()
        {
            Name = "STATus:QUEStionable:[EVENt]";
            VerdictTest = VerdictTestEnum.Ignore;
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            UpgradeVerdict(Verdict.NotSet);
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            _eventQR = myInstrument.Status.GetQuestionableEvent();

            int? result = _eventQR;

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
                Results.Publish("Event", new { Event = (int)_eventQR });
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xdStatusQuestionableInstrumentEventStep
    [Display("[EVENt]", Groups: new[] { "Keysight Instrument Basic Control", "E364x Dual Output DC Power Supplies", "STATus", "QUEStionable", "INSTrument" }, Description: "Query the Questionable Instrument Event register.")]
    public class E364xdStatusQuestionableInstrumentEventStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x Dual Output DC Power Supplies\Docs\e364x_77.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public E364xdInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Output]
        [Display(Group: "Query Response ", Name: "Event", Description: "Returns the Questionable Instrument Event register.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public int? _eventQR { get; private set; }
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
        public E364xdStatusQuestionableInstrumentEventStep()
        {
            Name = "STATus:QUEStionable:INSTrument:[EVENt]";
            VerdictTest = VerdictTestEnum.Ignore;
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            UpgradeVerdict(Verdict.NotSet);
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            _eventQR = myInstrument.Status.GetQuestionableInstrumentEvent();

            int? result = _eventQR;

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
                Results.Publish("Event", new { Event = (int)_eventQR });
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xdStatusQuestionableInstrumentEnableStep
    [Display("ENABle", Groups: new[] { "Keysight Instrument Basic Control", "E364x Dual Output DC Power Supplies", "STATus", "QUEStionable", "INSTrument" }, Description: "Set the value of the Questionable Instrument Enable register.  Return the value of the Questionable Instrument Enable register.")]
    public class E364xdStatusQuestionableInstrumentEnableStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x Dual Output DC Power Supplies\Docs\e364x_78.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public E364xdInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "Enablevalue", Description: "The value of the Questionable Instrument Enable register.", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public int? _enableValueCP { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "Enable", Description: "Return the value of the Questionable Instrument Enable register.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public int? _enableQR { get; private set; }
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
        public E364xdStatusQuestionableInstrumentEnableStep()
        {
            Name = "STATus:QUEStionable:INSTrument:ENABle";
            VerdictTest = VerdictTestEnum.Ignore;
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            UpgradeVerdict(Verdict.NotSet);
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
                    Results.Publish("Enable", new { Enable = (int)_enableQR });
                }
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xdStatusQuestionableInstrumentIsummaryConditionStep
    [Display("CONDition", Groups: new[] { "Keysight Instrument Basic Control", "E364x Dual Output DC Power Supplies", "STATus", "QUEStionable", "INSTrument", "ISUMmary" }, Description: "Return the CV or CC condition of the specified instrument.")]
    public class E364xdStatusQuestionableInstrumentIsummaryConditionStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x Dual Output DC Power Supplies\Docs\e364x_81.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public E364xdInstrument myInstrument { get; set; }
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
        [Display(Group: "Query Response ", Name: "Condition", Description: "Return the CV or CC condition of the specified instrument.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public int? _conditionQR { get; private set; }
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
        public E364xdStatusQuestionableInstrumentIsummaryConditionStep()
        {
            Name = "STATus:QUEStionable:INSTrument:ISUMmary:CONDition";
            VerdictTest = VerdictTestEnum.Ignore;
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            UpgradeVerdict(Verdict.NotSet);
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            _conditionQR = myInstrument.Status.GetQuestionableInstrumentIsummaryCondition(ISUMmarySuffix);

            int? result = _conditionQR;

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
                Results.Publish("Condition", new { Condition = (int)_conditionQR });
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xdStatusQuestionableInstrumentIsummaryEventStep
    [Display("[EVENt]", Groups: new[] { "Keysight Instrument Basic Control", "E364x Dual Output DC Power Supplies", "STATus", "QUEStionable", "INSTrument", "ISUMmary" }, Description: "Return the value of the Questionable Instrument Isummary Event register for a specific output of the two-output power supply.")]
    public class E364xdStatusQuestionableInstrumentIsummaryEventStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x Dual Output DC Power Supplies\Docs\e364x_80.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public E364xdInstrument myInstrument { get; set; }
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
        [Display(Group: "Query Response ", Name: "Event", Description: "Return the value of the Questionable Instrument Isummary Event register for a specific output of the two-output power supply.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public int? _eventQR { get; private set; }
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
        public E364xdStatusQuestionableInstrumentIsummaryEventStep()
        {
            Name = "STATus:QUEStionable:INSTrument:ISUMmary:[EVENt]";
            VerdictTest = VerdictTestEnum.Ignore;
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            UpgradeVerdict(Verdict.NotSet);
            int tempTimeout = myInstrument.IoTimeout;
            myInstrument.IoTimeout = timeout != null ? (int)timeout : myInstrument.IoTimeout;
            _eventQR = myInstrument.Status.GetQuestionableInstrumentIsummaryEvent(ISUMmarySuffix);

            int? result = _eventQR;

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
                Results.Publish("Event", new { Event = (int)_eventQR });
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xdStatusQuestionableInstrumentIsummaryEnableStep
    [Display("ENABle", Groups: new[] { "Keysight Instrument Basic Control", "E364x Dual Output DC Power Supplies", "STATus", "QUEStionable", "INSTrument", "ISUMmary" }, Description: "Set the value of the Questionable Instrument Isummary Enable register for a specific output of the two-output power supply.   This query returns the value of the Questionable Instrument Isummary Enable register.")]
    public class E364xdStatusQuestionableInstrumentIsummaryEnableStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x Dual Output DC Power Supplies\Docs\e364x_82.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public E364xdInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Settings", Name: "ISUMmary <n>", Description: "output identifier. (min: 1, max: 2)", Order: 25.1)]
        public int? ISUMmarySuffix { get; set; }
        [Display(Group: "Command Parameter ", Name: "Enablevalue", Description: "The value of the Questionable Instrument Isummary Enable register.", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public int? _enableValueCP { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "Enablevalue", Description: "Returns the current value of the Questionable Instrument Isummary Enable register.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public int? _enableValueQR { get; private set; }
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
        public E364xdStatusQuestionableInstrumentIsummaryEnableStep()
        {
            Name = "STATus:QUEStionable:INSTrument:ISUMmary:ENABle";
            VerdictTest = VerdictTestEnum.Ignore;
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            UpgradeVerdict(Verdict.NotSet);
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
                    Results.Publish("Enablevalue", new { Enablevalue = (int)_enableValueQR });
                }
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #endregion
    #region SYSTem Node Classes
    #region E364xdSystemBeeperImmediateStep
    [Display("[IMMediate]", Groups: new[] { "Keysight Instrument Basic Control", "E364x Dual Output DC Power Supplies", "SYSTem", "BEEPer" }, Description: "Issue a single beep immediately.")]
    public class E364xdSystemBeeperImmediateStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x Dual Output DC Power Supplies\Docs\e364x_50.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public E364xdInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xdSystemBeeperImmediateStep()
        {
            Name = "SYSTem:BEEPer:[IMMediate]";
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
    #region E364xdSystemErrorStep
    [Display("ERRor", Groups: new[] { "Keysight Instrument Basic Control", "E364x Dual Output DC Power Supplies", "SYSTem" }, Description: "Query the power supply’s error queue.")]
    public class E364xdSystemErrorStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x Dual Output DC Power Supplies\Docs\e364x_51.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public E364xdInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Output]
        [Display(Group: "Query Response ", Name: "Erorr", Description: "Erorr numbers.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public int? _erorrQR { get; private set; }
        [Output]
        [Display(Group: "Query Response ", Name: "Message", Description: "Messages.", Order: 50.2)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public string _messageQR { get; private set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xdSystemErrorStep()
        {
            Name = "SYSTem:ERRor";
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
    #region E364xdSystemInterfaceStep
    [Display("INTerface", Groups: new[] { "Keysight Instrument Basic Control", "E364x Dual Output DC Power Supplies", "SYSTem" }, Description: "Select the remote interface.")]
    public class E364xdSystemInterfaceStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x Dual Output DC Power Supplies\Docs\e364x_70.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public E364xdInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "Interface Custom Input", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public bool _interfaceCPCustomInput { get; set; }
        [Display(Group: "Command Parameter ", Name: "Interface", Description: "The remote interface.", Order: 30.2)]
        [EnabledIf("_interfaceCPCustomInput", false, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public gpibRs232 _interfaceCP { get; set; }
        [Display(Group: "Command Parameter ", Name: "Interface", Description: "The remote interface.", Order: 30.3)]
        [EnabledIf("_interfaceCPCustomInput", true, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public string _interfaceCPStr { get; set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xdSystemInterfaceStep()
        {
            Name = "SYSTem:INTerface";
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
    #region E364xdSystemLocalStep
    [Display("LOCal", Groups: new[] { "Keysight Instrument Basic Control", "E364x Dual Output DC Power Supplies", "SYSTem" }, Description: "Place the power supply in the local mode during RS-232 operation.")]
    public class E364xdSystemLocalStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x Dual Output DC Power Supplies\Docs\e364x_71.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public E364xdInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xdSystemLocalStep()
        {
            Name = "SYSTem:LOCal";
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
    #region E364xdSystemRemoteStep
    [Display("REMote", Groups: new[] { "Keysight Instrument Basic Control", "E364x Dual Output DC Power Supplies", "SYSTem" }, Description: "Place the power supply in the remote mode for RS-232 operation.")]
    public class E364xdSystemRemoteStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x Dual Output DC Power Supplies\Docs\e364x_72.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public E364xdInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xdSystemRemoteStep()
        {
            Name = "SYSTem:REMote";
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
    #region E364xdSystemRwlockStep
    [Display("RWLock", Groups: new[] { "Keysight Instrument Basic Control", "E364x Dual Output DC Power Supplies", "SYSTem" }, Description: "Place the power supply in the remote mode for RS-232 operation.")]
    public class E364xdSystemRwlockStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x Dual Output DC Power Supplies\Docs\e364x_73.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public E364xdInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xdSystemRwlockStep()
        {
            Name = "SYSTem:RWLock";
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
    #region E364xdSystemVersionStep
    [Display("VERSion", Groups: new[] { "Keysight Instrument Basic Control", "E364x Dual Output DC Power Supplies", "SYSTem" }, Description: "Query the power supply to determine the present SCPI version.")]
    public class E364xdSystemVersionStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x Dual Output DC Power Supplies\Docs\e364x_52.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public E364xdInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Output]
        [Display(Group: "Query Response ", Name: "Version", Description: "Returns the power supply to determine the present SCPI version.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public string _versionQR { get; private set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xdSystemVersionStep()
        {
            Name = "SYSTem:VERSion";
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
    #region E364xdTriggerSequenceDelayStep
    [Display("DELay", Groups: new[] { "Keysight Instrument Basic Control", "E364x Dual Output DC Power Supplies", "TRIGger", "[SEQuence]" }, Description: "Set the time delay between the detection of an event on the specified trigger source and the start of any corresponding trigger action on the power supply output.  Query the trigger delay.")]
    public class E364xdTriggerSequenceDelayStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x Dual Output DC Power Supplies\Docs\e364x_34.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public E364xdInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "Seconds Custom Input", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public bool _secondsCPCustomInput { get; set; }
        [Display(Group: "Command Parameter ", Name: "Seconds", Description: "The trigger delay.", Order: 30.2)]
        [EnabledIf("_secondsCPCustomInput", false, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public minMax _secondsCP { get; set; }
        [Display(Group: "Command Parameter ", Name: "Seconds", Description: "The trigger delay.", Order: 30.3)]
        [EnabledIf("_secondsCPCustomInput", true, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public string _secondsCPStr { get; set; }
        [Display(Group: "Query Parameter ", Name: "Preset Custom Input", Order: 40.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public bool _presetQPCustomInput { get; set; }
        [Display(Group: "Query Parameter ", Name: "Preset", Description: "MINimum | MAXimum", Order: 40.2)]
        [EnabledIf("_presetQPCustomInput", false, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public minMax _presetQP { get; set; }
        [Display(Group: "Query Parameter ", Name: "Preset", Description: "MINimum | MAXimum", Order: 40.3)]
        [EnabledIf("_presetQPCustomInput", true, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public string _presetQPStr { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "Seconds", Description: "Returns the current value of the trigger delay.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public double? _secondsQR { get; private set; }
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
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xdTriggerSequenceDelayStep()
        {
            Name = "TRIGger:[SEQuence]:DELay";
            VerdictTest = VerdictTestEnum.Ignore;
        }
        #endregion
        #region Run Method
        public override void Run()
        {
            UpgradeVerdict(Verdict.NotSet);
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
                    Results.Publish("Seconds", new { Seconds = (double)_secondsQR });
                }
            }
            myInstrument.IoTimeout = tempTimeout;
        }
        #endregion
    }

    #endregion
    #region E364xdTriggerSequenceSourceStep
    [Display("SOURce", Groups: new[] { "Keysight Instrument Basic Control", "E364x Dual Output DC Power Supplies", "TRIGger", "[SEQuence]" }, Description: "Select the source from which the power supply will accept a trigger.  Query the present trigger source.")]
    public class E364xdTriggerSequenceSourceStep : TestStep
    {
        #region Help Button
        [Display("Detailed Help")]
        [Browsable(true)]
        public void OpenHelpLink()
        {
            CommonMethods.OpenHelpLink(@"\E364x Dual Output DC Power Supplies\Docs\e364x_36.html");
        }
        #endregion
        #region Instrument
        [Display(Group: "Settings", Name: "Select Instrument", Order: 10.1)]
        public E364xdInstrument myInstrument { get; set; }
        #endregion
        #region SetAction
        public enum SetAction { Command, Query }
        [Display(Group: "Settings", Name: "Operation Type", Order: 20.1)]
        public SetAction Action { get; set; }
        #endregion
        #region Parameter/Response/Suffix Properties
        [Display(Group: "Command Parameter ", Name: "Source Custom Input", Order: 30.1)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public bool _sourceCPCustomInput { get; set; }
        [Display(Group: "Command Parameter ", Name: "Source", Description: "The trigger source.", Order: 30.2)]
        [EnabledIf("_sourceCPCustomInput", false, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public busImmediate _sourceCP { get; set; }
        [Display(Group: "Command Parameter ", Name: "Source", Description: "The trigger source.", Order: 30.3)]
        [EnabledIf("_sourceCPCustomInput", true, HideIfDisabled = true)]
        [EnabledIf("Action", SetAction.Command, HideIfDisabled = true)]

        public string _sourceCPStr { get; set; }
        [Output]
        [Display(Group: "Query Response ", Name: "Source", Description: "Returns the present trigger source.", Order: 50.1)]
        [EnabledIf("Action", SetAction.Query, HideIfDisabled = true)]

        public busImmediate _sourceQR { get; private set; }
        #endregion
        #region Timeout
        [Display(Group: "Timeout", Name: "I/O Timeout", Description: "I/O timeout duration in milliseconds (Instrument value if Empty)", Order: 70.1)]
        public int? timeout { get; set; }
        #endregion
        #region Constructor
        public E364xdTriggerSequenceSourceStep()
        {
            Name = "TRIGger:[SEQuence]:SOURce";
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