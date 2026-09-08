namespace HolyTac.Gateway.Auth;

public class AuthOptions
{
    public const string SectionName = "Auth";

    public JwtOptions Jwt { get; set; } = new();
    public List<StaffAccount> Accounts { get; set; } = [];
}

public class JwtOptions
{
    public string Issuer { get; set; } = default!;
    public string Audience { get; set; } = default!;
    public string SigningKey { get; set; } = default!;
    public int ExpiryMinutes { get; set; } = 480;
}

/// <summary>Cuenta de staff configurada. Password nunca se guarda en claro, solo su hash PBKDF2.</summary>
public class StaffAccount
{
    public string Username { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;

    /// <summary>Mesero, Cocina o Gerente — ver <see cref="StaffRoles"/>.</summary>
    public string Role { get; set; } = default!;
}

/// <summary>Los 3 roles de staff del roadmap (Fase 2). "role" es también el nombre del claim en el JWT.</summary>
public static class StaffRoles
{
    public const string Mesero = "Mesero";
    public const string Cocina = "Cocina";
    public const string Gerente = "Gerente";

    public static readonly IReadOnlySet<string> All = new HashSet<string>([Mesero, Cocina, Gerente]);
}
