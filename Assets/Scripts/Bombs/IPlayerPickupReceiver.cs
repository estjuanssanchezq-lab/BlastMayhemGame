// Cualquier objeto que quiera funcionar como Player
// debe tener estos métodos.

// Define lo que la clase que la usa debe hacer, sin importar
// cómo lo haga. Es como un contrato que dice:
// "Si quieres ser un Player, debes tener este método".

public interface IPickupReceiver
{
    void AddBomb();
}
