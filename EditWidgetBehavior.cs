﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Curvature
{
    public partial class EditWidgetBehavior : UserControl, IInputBroker
    {
        private Behavior EditBehavior;


        internal EditWidgetBehavior(Behavior behavior)
        {
            InitializeComponent();
            EditBehavior = behavior;

            BehaviorNameLabel.Text = "Behavior: " + EditBehavior.ReadableName;

            var inputs = new HashSet<InputAxis>();
            foreach (var consideration in EditBehavior.Considerations)
            {
                inputs.Add(consideration.Input);

                ScoreLayoutPanel.Controls.Add(new EditWidgetConsiderationScore(consideration, this));
            }

            foreach (var input in inputs)
            {
                var widget = new EditWidgetConsiderationInput(input, this);
                widget.Tag = input;
                InputFlowPanel.Controls.Add(widget);
            }

            RefreshInputs();
        }

        public float GetInputValue(InputAxis axis)
        {
            foreach (EditWidgetConsiderationInput input in InputFlowPanel.Controls)
            {
                if (input.Tag == axis)
                    return input.GetNormalizedValue();
            }

            return 0.0f;
        }

        public void RefreshInputs()
        {
            ScoreListView.Items.Clear();

            double finalScore = (double)BehaviorWeightEditBox.Value;
            var weightItem = new ListViewItem(new string[] { $"{finalScore:f3}", "Behavior weight" });
            weightItem.Group = ScoreListView.Groups[1];

            ScoreListView.Items.Add(weightItem);
     
            foreach (EditWidgetConsiderationScore score in ScoreLayoutPanel.Controls)
            {
                score.Refresh();

                string considerationName = score.GetName();
                double considerationScore = score.GetValue();

                var item = new ListViewItem(new string[] { $"{considerationScore:f3}", $"{considerationName}" });
                item.Group = ScoreListView.Groups[0];

                ScoreListView.Items.Add(item);
                finalScore *= considerationScore;
            }

            if (MomentumBonusCheckBox.Checked)
            {
                finalScore *= 1.25;

                var item = new ListViewItem(new string[] { "1.250", "Momentum bonus" });
                item.Group = ScoreListView.Groups[1];

                ScoreListView.Items.Add(item);
            }

            FinalScoreLabel.Text = $"Final Score = {finalScore:f3}";
        }

        private void MomentumBonusCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            RefreshInputs();
        }

        private void BehaviorWeightEditBox_ValueChanged(object sender, EventArgs e)
        {
            RefreshInputs();
        }
    }
}