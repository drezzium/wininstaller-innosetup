﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FreenetTray
{
    public partial class PreferencesWindow : Form
    {
        private const int StartIconIndex = 0;
        private const int StartFreenetIndex = 1;

        private readonly string RegistryStartupName = "Freenet";

        public PreferencesWindow()
        {
            InitializeComponent();

            StartupCheckboxList.SetItemChecked(StartIconIndex,
                                               Properties.Settings.Default.StartIcon);
            StartupCheckboxList.SetItemChecked(StartFreenetIndex,
                                               Properties.Settings.Default.StartFreenet);
        }

        private void Apply_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.StartIcon = StartupCheckboxList.GetItemChecked(StartIconIndex);
            Properties.Settings.Default.StartFreenet = StartupCheckboxList.GetItemChecked(StartFreenetIndex);
            Properties.Settings.Default.Save();

            using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
            {
                // TODO: Assuming startup registry location exists. Is this viable?
                key.DeleteValue(RegistryStartupName, false);
                // Double quotes are required around the executable path to get multiple(?) command line arguments.
                if (Properties.Settings.Default.StartIcon)
                {
                    if (Properties.Settings.Default.StartFreenet)
                    {
                        // Display icon and open Freenet
                        key.SetValue(RegistryStartupName, '"' + Application.ExecutablePath + "\" -start");
                    }
                    else
                    {
                        // Just display icon
                        key.SetValue(RegistryStartupName, Application.ExecutablePath);
                    }
                }
                else if (Properties.Settings.Default.StartFreenet)
                {
                    // Start Freenet and hide icon (which closes the tray app)
                    key.SetValue(RegistryStartupName, '"' + Application.ExecutablePath + "\" -start -hide");
                }
            }

            Close();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}