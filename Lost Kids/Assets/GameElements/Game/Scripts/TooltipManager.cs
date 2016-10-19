using UnityEngine;
using System.Collections;

public static class TooltipManager {
    // Evento para advertir del cambio en el TooltipManager
    public delegate void TooltipManagerChanged(bool on);
    public static TooltipManagerChanged TooltipOnOff;
    public static bool On {
        get {
            return _on;
        }
        set {
            _on = value;
            if (TooltipOnOff != null) {
                TooltipOnOff(_on);
            }
        }
    }
    private static bool _on = true;
}
