using System;
using StedySoft.SenseSDK;

namespace TrainStationAdvisor.ClassLibrary
{
    public class SenseFactory
    {
        public class SensePropertyBag
        {
            public string PropertyName { get; set; }
            public string PropertyLabel { get; set; }
            public string PropertyExplanation{ get; set; }
            public bool PropertyEnabled { get; set; }

            public SensePropertyBag(string propertyName, 
                                    string propertyLabel, 
                                    string propertyExplanation,
                                    bool propertyEnabled)
            {
                PropertyName = propertyName;
                PropertyLabel = propertyLabel;
                PropertyExplanation = propertyExplanation;
                PropertyEnabled = propertyEnabled;
            }

            public SensePropertyBag(string propertyName, 
                                    string propertyLabel, 
                                    string propertyExplanation) : 
                this(propertyName, propertyLabel, propertyExplanation, true)
            {}

            public SensePropertyBag(string propertyName,
                                    string propertyLabel,
                                    bool propertyEnabled) :
                this(propertyName, propertyLabel, string.Empty, propertyEnabled)
            { }

            public SensePropertyBag(string propertyName,
                                    string propertyLabel) :
                this(propertyName, propertyLabel, string.Empty)
            { }
        }

        public static SensePanelNumericItem CreateNumericItem(SensePropertyBag parameter,
                                                              SensePanelNumericItem.ValueChangedEventHandler handler)
        {
            SensePanelNumericItem result = new SensePanelNumericItem(parameter.PropertyName) 
            { 
                AutoShowDialog = true, 
                AutoShowDialogText = parameter.PropertyLabel, 
                ButtonAnimation = false,
                PrimaryText = parameter.PropertyLabel,
                SecondaryText = parameter.PropertyExplanation,
                Value = int.Parse(Settings.GetProperty(parameter.PropertyName))
            };
            result.OnValueChanged += handler;
                
            return result;
        }

        public static SensePanelTrackbarItem CreateTrackbarItem(SensePropertyBag parameter, 
                                                                SensePanelTrackbarItem.ValueEventHandler handler)
        {
            SensePanelTrackbarItem result = new SensePanelTrackbarItem(parameter.PropertyName)
            {
                ButtonAnimation = true,
                ProcessEntireButton = false,
                Text = parameter.PropertyLabel,
                Value = int.Parse(Settings.GetProperty(parameter.PropertyName))
            };
            result.OnValueChanged += handler;

            return result;
        }

        public static SensePanelComboItem CreateComboItem(SensePropertyBag parameter, 
                                                          SensePanelComboItem.SelectedIndexChangedEventHandler handler)
        {
            SensePanelComboItem result = new SensePanelComboItem(parameter.PropertyName)
            {
                LabelText = parameter.PropertyLabel,
                Enabled = parameter.PropertyEnabled
            };

            result.OnSelectedIndexChanged += handler;
                
            return result;
        }

        public static SensePanelCheckboxItem CreateCheckboxItem(SensePropertyBag parameter,
                                                                SensePanelCheckboxItem.StatusEventHandler handler)
        {
            SensePanelCheckboxItem result = new SensePanelCheckboxItem(parameter.PropertyName)
            {
                ButtonAnimation = true,
                ProcessEntireButton = false,
                PrimaryText = parameter.PropertyLabel,
                Enabled = true,
                Status = BoolToStatus(Convert.ToBoolean(Settings.GetProperty(parameter.PropertyName))),
                SecondaryText = parameter.PropertyExplanation
            };
            result.OnStatusChanged += handler;

            return result;
        }

        public static SensePanelTextboxItem CreateTextboxItem(SensePropertyBag parameter, EventHandler handler)
        {
            SensePanelTextboxItem result = new SensePanelTextboxItem(parameter.PropertyName)
            {
                LabelWidth = 30,
                LayoutSytle = SenseTexboxLayoutStyle.Horizontal,
                LabelText = parameter.PropertyLabel,
                ShowSeparator = true,
                Text = Settings.GetProperty(parameter.PropertyName)
            };
            result.TextChanged += handler;

            return result;
        }

        public static SensePanelOnOffItem CreateOnOffItem(SensePropertyBag parameter,
                                                          SensePanelOnOffItem.StatusEventHandler handler)
        {
            SensePanelOnOffItem result = new SensePanelOnOffItem(parameter.PropertyName)
            {
                ButtonAnimation = true,
                ProcessEntireButton = false,
                PrimaryText = parameter.PropertyLabel,
                Enabled = true,
                Status = BoolToStatus(Convert.ToBoolean(Settings.GetProperty(parameter.PropertyName))),
                SecondaryText = parameter.PropertyExplanation
            };
            result.OnStatusChanged += handler;

            return result;
        }

        public static ItemStatus BoolToStatus(bool value)
        {
            return (value ? ItemStatus.On : ItemStatus.Off);
        }

        public static bool StatusToBool(ItemStatus value)
        {
            return (value == ItemStatus.On);
        }


    }
}
