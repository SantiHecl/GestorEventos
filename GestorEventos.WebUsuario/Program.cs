using GestorEventos.Servicios.Entidades;
using GestorEventos.Servicios.Servicios;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.Extensions.Options;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IEventoService, EventoService>();
builder.Services.AddScoped<IPersonaService, PersonaService>();
builder.Services.AddScoped<IServicioService, ServicioService>();
builder.Services.AddScoped<IEventosServiciosService, EventosServiciosService>();

 
builder.Services.AddAuthentication(opciones =>
{
    opciones.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    opciones.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    opciones.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle(GoogleDefaults.AuthenticationScheme, opciones =>
{
    opciones.ClientId = builder.Configuration.GetSection("GoogleKeys:ClientId").Value + ".apps.googleusercontent.com";
    opciones.ClientSecret = builder.Configuration.GetSection("GoogleKeys:ClientPriv").Value;
 
    opciones.Events.OnCreatingTicket = ctx =>
    {
        var usuarioServicio = ctx.HttpContext.RequestServices.GetRequiredService<IUsuarioService>();

        string googleNameIdentifier = ctx.Identity.Claims.First(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value.ToString(); ;

        var usuario = usuarioServicio.GetUsuarioPorGoogleSubject(googleNameIdentifier);
        int idUsuario = 0;
        if (usuario == null)
        {
            Usuario usuarioNuevo = new Usuario();
            usuarioNuevo.GoogleIdentificador = googleNameIdentifier;
            usuarioNuevo.NombreCompleto = ctx.Identity.Claims.First(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name").Value.ToString();
            usuarioNuevo.Nombre = ctx.Identity.Claims.First(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname").Value.ToString();
            usuarioNuevo.Apellido = ctx.Identity.Claims.First(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname").Value.ToString();            
            usuarioNuevo.Email = ctx.Identity.Claims.First(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value.ToString();
            usuarioNuevo.Borrado = false;

            idUsuario = usuarioServicio.AgregarNuevoUsuario(usuarioNuevo);
        }
        else
        {
            idUsuario = usuario.IdUsuario;
        }
        //ctx.Identity.
        //usuarioServicio.GetUsuarioPorGoogleSubject(ctx.Identity.Claims)
        // Agregar reclamaciones personalizadas aqu�
        //ctx.Identity.AddClaim(new System.Security.Claims.Claim("usuarioSolterout", idUsuario.ToString()));

        //var accessToken = ctx.AccessToken;
        //ctx.Identity.AddClaim(new System.Security.Claims.Claim("accessToken", accessToken));

        ctx.Identity.AddClaim(new Claim("usuarioSolterout", idUsuario.ToString()));
        ctx.Identity.AddClaim(new Claim("accessToken", ctx.AccessToken));

        // Actualizar la cookie de autenticaci�n
        var claimsIdentity = new ClaimsIdentity(ctx.Identity.Claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        ctx.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

        return Task.CompletedTask;
    };
});

var app = builder.Build();
 
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

    app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
