namespace RealEstateApp.Core.Models;

public enum StaffActionStatus
{
    Success,
    NotFound,

    // Autoproteccion (solo aplica a Administrador, nunca a Desarrollador): el
    // admin autenticado no puede editar ni inactivar su propio usuario.
    CannotModifySelf,

    // Debe existir siempre >= 1 administrador activo en el sistema.
    LastActiveAdmin,

    // La nueva contrasena (opcional, en edicion) no cumplio las reglas de
    // Identity (complejidad, longitud, etc.) -- ver StaffUpdateResult.Errors.
    PasswordUpdateFailed
}
