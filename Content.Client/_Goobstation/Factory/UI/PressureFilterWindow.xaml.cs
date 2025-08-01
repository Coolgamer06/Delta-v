// SPDX-FileCopyrightText: 2025 deltanedas <@deltanedas:kde.org>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Client.UserInterface.Controls;
using Content.Shared._Goobstation.Factory.Filters;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.XAML;

namespace Content.Client._Goobstation.Factory.UI;

[GenerateTypedNameReferences]
public sealed partial class PressureFilterWindow : FancyWindow
{
    [Dependency] private readonly EntityManager _entMan = default!;

    public event Action<float>? OnSetMin;
    public event Action<float>? OnSetMax;

    private float _min, _max;

    public PressureFilterWindow()
    {
        IoCManager.InjectDependencies(this);
        RobustXamlLoader.Load(this);

        MinEdit.OnTextChanged += _ => UpdateButtons();

        MinConfirmButton.OnPressed += _ =>
        {
            if (float.TryParse(MinEdit.Text, out var min))
                OnSetMin?.Invoke(min);
        };

        MaxEdit.OnTextChanged += _ => UpdateButtons();

        MaxConfirmButton.OnPressed += _ =>
        {
            if (float.TryParse(MaxEdit.Text, out var max))
                OnSetMax?.Invoke(max);
        };

        OnSetMin += min => { _min = min; UpdateButtons(); };
        OnSetMax += max => { _max = max; UpdateButtons(); };
    }

    public void SetEntity(EntityUid uid)
    {
        if (!_entMan.TryGetComponent<PressureFilterComponent>(uid, out var comp))
            return;

        _min = comp.Min;
        _max = comp.Max;
        MinEdit.Text = _min.ToString();
        MaxEdit.Text = _max.ToString();
        UpdateButtons();
    }

    private void UpdateButtons()
    {
        MinConfirmButton.Disabled = !float.TryParse(MinEdit.Text, out var min) || min < 0f || min > _max || min == _min;
        MaxConfirmButton.Disabled = !float.TryParse(MaxEdit.Text, out var max) || max < _min || max == _max;
    }
}
