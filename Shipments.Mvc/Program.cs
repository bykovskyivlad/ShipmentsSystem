using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

//
// =====================
// SERVICES
// =====================
//

// MVC
builder.Services.AddControllersWithViews();

// HttpClient → API
builder.Services.AddHttpClient("Api", client =>
{
    var baseUrl = builder.Configuration["Api:BaseUrl"];
    client.BaseAddress = new Uri(baseUrl!);
});

// potrzebne dla ApiClient (JWT z cookie)
builder.Services.AddHttpContextAccessor();

// ApiClient (MVC → API)
builder.Services.AddScoped<ApiClient>();

// Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/Login";

        // bezpieczeństwo (HTTPS)
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.HttpOnly = true;
    });

// Authorization (role)
builder.Services.AddAuthorization();


//
// =====================
// APP
// =====================
//

var app = builder.Build();


//
// =====================
// MIDDLEWARE
// =====================
//

// errors
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


//
// =====================
// ROUTING
// =====================
//

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
