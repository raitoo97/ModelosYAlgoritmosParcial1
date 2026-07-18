using UnityEngine;
public class ScreenPause : ScreenBase
{
    //Ademas de ser el marcador que usa UIController para no instanciar muchas pausas,
    //ahora se encarga de mostrar el cursor cuando se activa.
    public override void Activate()
    {
        base.Activate();
        Cursor.visible = true;
    }
}
