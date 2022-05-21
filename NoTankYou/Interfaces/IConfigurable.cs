using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Interface;
using ImGuiNET;
using ImGuiScene;
using NoTankYou.Components;
using NoTankYou.Localization;
using NoTankYou.Utilities;

namespace NoTankYou.Interfaces
{
    internal interface IConfigurable : ITabItem
    {
        string ConfigurationPaneLabel { get; }
        string AboutInformationBox { get; }
        string TechnicalInformation { get; }
        TextureWrap? AboutImage { get; }

        void DrawOptions();

        void DrawBaseOptions() => DrawOptions();

        void ITabItem.DrawConfigurationPane()
        {
            var contentWidth = ImGui.GetContentRegionAvail().X;
            var textWidth = ImGui.CalcTextSize(ConfigurationPaneLabel).X;
            var textStart = contentWidth / 2.0f - textWidth / 2.0f;

            ImGui.SetCursorPos(ImGui.GetCursorPos() with {X = textStart});
            ImGui.Text(ConfigurationPaneLabel);

            ImGui.Spacing();

            DrawTabs();
        }

        void DrawTabs()
        {
            if (ImGui.BeginTabBar("SelectionPaneTabBar", ImGuiTabBarFlags.None))
            {
                if (ImGui.BeginTabItem(Strings.Common.Labels.About))
                {
                    if (ImGui.BeginChild("AboutContentsChild", ImGui.GetContentRegionAvail(), false,
                            ImGuiWindowFlags.AlwaysVerticalScrollbar))
                    {
                        DrawAboutContents();
                    }

                    ImGui.EndChild();

                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem(Strings.Common.Labels.Options))
                {
                    if (ImGui.BeginChild("OptionsContentsChild", ImGui.GetContentRegionAvail(), false,
                            ImGuiWindowFlags.AlwaysVerticalScrollbar))
                    {
                        DrawBaseOptions();
                    }

                    ImGui.EndChild();

                    ImGui.EndTabItem();
                }

                ImGui.EndTabBar();
            }
        }

        void DrawAboutContents()
        {
            var region = ImGui.GetContentRegionAvail();
            var currentPosition = ImGui.GetCursorPos();

            if (AboutImage != null)
            {
                var imageRatio = (float) AboutImage.Height / AboutImage.Width;
                var width = region.X * 0.80f;
                var height = width * imageRatio;
                var imageSize = new Vector2(width, height);
                var insetVector = new Vector2(2.5f);

                ImGui.SetCursorPos(currentPosition with
                {
                    X = region.X / 2.0f - imageSize.X / 2.0f,
                    Y = currentPosition.Y + 10.0f * ImGuiHelpers.GlobalScale
                });
                var startPosition = ImGui.GetCursorScreenPos();
                var imageStart = startPosition + insetVector;
                var imageStop = startPosition + imageSize - insetVector;

                ImGui.GetWindowDrawList().AddRectFilled(startPosition, startPosition + imageSize,
                    ImGui.GetColorU32(Colors.White), 40.0f);

                ImGui.GetWindowDrawList().AddImageRounded(AboutImage.ImGuiHandle, imageStart, imageStop, Vector2.Zero,
                    Vector2.One, ImGui.GetColorU32(Colors.White), 40.0f);

                ImGui.SetCursorScreenPos(startPosition + imageSize);
                ImGuiHelpers.ScaledDummy(20.0f);
            }

            ImGui.PushStyleColor(ImGuiCol.Text, Colors.Grey);

            new InfoBox
            {
                Label = Strings.Common.Labels.Description,
                ContentsAction = () => { ImGui.Text(AboutInformationBox); }
            }.DrawCentered();
            ImGuiHelpers.ScaledDummy(30.0f);

            new InfoBox
            {
                Label = Strings.Common.Labels.TechnicalDescription,
                ContentsAction = () => { ImGui.Text(TechnicalInformation); }
            }.DrawCentered();
            ImGuiHelpers.ScaledDummy(20.0f);

            ImGui.PopStyleColor();
        }

    }
}
