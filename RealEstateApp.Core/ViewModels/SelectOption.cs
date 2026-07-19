namespace RealEstateApp.Core.ViewModels;

// Reemplaza a Microsoft.AspNetCore.Mvc.Rendering.SelectListItem dentro de
// Core (ese tipo pertenece al framework web; Core no debe depender de el).
// La vista/controlador en WebApp lo convierte a SelectListItem si hace falta.
public class SelectOption
{
    public string Value { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}
