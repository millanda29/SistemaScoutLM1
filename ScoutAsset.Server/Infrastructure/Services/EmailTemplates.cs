using System;
using System.IO;

namespace ScoutAsset.Server.Infrastructure.Services;

public static class EmailTemplates
{
    private static string GetLogoBase64(string fileName)
    {
        try
        {
            var pathsToTry = new[]
            {
                Path.Combine(Directory.GetCurrentDirectory(), "..", "scoutasset.client", "public", fileName),
                Path.Combine(Directory.GetCurrentDirectory(), "public", fileName),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "public", fileName),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "scoutasset.client", "public", fileName)
            };

            foreach (var path in pathsToTry)
            {
                if (File.Exists(path))
                {
                    var bytes = File.ReadAllBytes(path);
                    return $"data:image/jpeg;base64,{Convert.ToBase64String(bytes)}";
                }
            }
        }
        catch { }

        return string.Empty;
    }

    public static string BuildBaseTemplate(string title, string contentHtml)
    {
        var logoNacional = GetLogoBase64("logoNacional.jpg");
        var logoGrupo = GetLogoBase64("logoGrupo.jpg");

        var logoNacionalImg = !string.IsNullOrEmpty(logoNacional)
            ? $"<img src='{logoNacional}' alt='Scouts del Ecuador' style='height: 52px; width: auto; object-fit: contain; filter: drop-shadow(0 2px 6px rgba(0,0,0,0.3));' />"
            : "<span style='color:#ffffff; font-weight:800; font-size:14px;'>⚜️ SCOUTS ECUADOR</span>";

        var logoGrupoImg = !string.IsNullOrEmpty(logoGrupo)
            ? $"<img src='{logoGrupo}' alt='Grupo Scout Leonardo Murialdo N.° 1' style='height: 52px; width: 52px; border-radius: 50%; object-fit: cover; border: 2px solid rgba(255,255,255,0.6); filter: drop-shadow(0 2px 6px rgba(0,0,0,0.3));' />"
            : "<span style='color:#ffd54f; font-weight:800; font-size:14px;'>N.° 1 MURIALDO</span>";

        return $@"
<!DOCTYPE html>
<html lang='es'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>{title}</title>
    <style>
        @import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;500;600;700;800&display=swap');
        
        body {{
            font-family: 'Plus Jakarta Sans', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif;
            background-color: #f3f0f9;
            color: #2d2345;
            margin: 0;
            padding: 0;
            -webkit-font-smoothing: antialiased;
        }}
        .wrapper {{
            width: 100%;
            background-color: #f3f0f9;
            padding: 40px 16px;
            box-sizing: border-box;
        }}
        .email-container {{
            max-width: 580px;
            margin: 0 auto;
            background-color: #ffffff;
            border-radius: 20px;
            overflow: hidden;
            box-shadow: 0 12px 32px rgba(97, 43, 175, 0.12);
            border: 1px solid #e7dff5;
        }}
        .header {{
            background: linear-gradient(135deg, #1d0742 0%, #4a148c 50%, #7c4dff 100%);
            padding: 36px 28px;
            text-align: center;
            position: relative;
        }}
        .logo-bar {{
            display: flex;
            justify-content: space-between;
            align-items: center;
            max-width: 480px;
            margin: 0 auto 20px;
            padding: 0 10px;
        }}
        .header h1 {{
            color: #ffffff;
            margin: 0;
            font-size: 26px;
            font-weight: 800;
            letter-spacing: -0.5px;
        }}
        .header p {{
            color: #e0d4fc;
            margin: 6px 0 0;
            font-size: 13.5px;
            font-weight: 500;
            letter-spacing: 0.3px;
        }}
        .content {{
            padding: 36px 32px;
            line-height: 1.65;
        }}
        .content h2 {{
            color: #1a083b;
            font-size: 22px;
            margin-top: 0;
            margin-bottom: 16px;
            font-weight: 800;
            letter-spacing: -0.3px;
        }}
        .content p {{
            font-size: 15px;
            color: #4a3e68;
            margin-bottom: 18px;
        }}
        .credential-card {{
            background: linear-gradient(145deg, #1b0c38 0%, #2e105e 100%);
            border-radius: 16px;
            padding: 24px;
            margin: 24px 0;
            box-shadow: 0 8px 24px rgba(46, 16, 94, 0.25);
            border: 1px solid #5c2bb8;
        }}
        .credential-header {{
            color: #ffd54f;
            font-size: 13px;
            font-weight: 700;
            text-transform: uppercase;
            letter-spacing: 1px;
            margin-bottom: 16px;
            display: flex;
            align-items: center;
        }}
        .credential-row {{
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 10px 0;
            border-bottom: 1px dashed rgba(255, 255, 255, 0.12);
        }}
        .credential-row:last-child {{
            border-bottom: none;
        }}
        .credential-label {{
            color: #b3a2d8;
            font-size: 13.5px;
            font-weight: 500;
        }}
        .credential-value {{
            color: #ffffff;
            font-size: 14.5px;
            font-weight: 700;
            font-family: 'Courier New', Courier, monospace;
        }}
        .password-badge {{
            background: #ffca28;
            color: #1b0c38;
            padding: 6px 14px;
            border-radius: 8px;
            font-weight: 800;
            letter-spacing: 1px;
            font-size: 15px;
            box-shadow: 0 2px 8px rgba(255, 202, 40, 0.4);
        }}
        .button-wrapper {{
            text-align: center;
            margin: 32px 0 24px;
        }}
        .button {{
            display: inline-block;
            padding: 14px 36px;
            background: linear-gradient(135deg, #7c4dff 0%, #512da8 100%);
            color: #ffffff !important;
            text-decoration: none;
            font-weight: 700;
            font-size: 15px;
            border-radius: 12px;
            box-shadow: 0 6px 20px rgba(124, 77, 255, 0.35);
            transition: all 0.2s ease;
        }}
        .security-alert {{
            background-color: #fff9e6;
            border-left: 4px solid #ffb300;
            border-radius: 10px;
            padding: 16px 20px;
            margin: 24px 0;
        }}
        .security-alert p {{
            margin: 0;
            font-size: 13.5px;
            color: #7f5f00;
            font-weight: 500;
        }}
        .footer {{
            background-color: #faf8fc;
            padding: 28px 32px;
            text-align: center;
            border-top: 1px solid #ede7f6;
            font-size: 12.5px;
            color: #8c7ba8;
        }}
        .footer strong {{
            color: #3b285c;
            font-size: 13.5px;
        }}
        .footer p {{
            margin: 4px 0;
        }}
    </style>
</head>
<body>
    <div class='wrapper'>
        <div class='email-container'>
            <div class='header'>
                <div class='logo-bar'>
                    <div>{logoNacionalImg}</div>
                    <div style='font-size: 26px;'>⚜️</div>
                    <div>{logoGrupoImg}</div>
                </div>
                <h1>ScoutAsset</h1>
                <p>Grupo Scout Leonardo Murialdo N.° 1 — Archidona</p>
            </div>
            <div class='content'>
                {contentHtml}
            </div>
            <div class='footer'>
                <p><strong>Grupo Scout Leonardo Murialdo N.° 1</strong></p>
                <p>Archidona — Napo, Ecuador</p>
                <p style='margin-top: 12px; font-size: 11.5px; color: #b0a3c7;'>Este es un correo automático del sistema ScoutAsset. Por favor, no respondas a este mensaje.</p>
            </div>
        </div>
    </div>
</body>
</html>";
    }

    public static string GetWelcomeEmail(string name, string username, string email, string tempPassword)
    {
        var content = $@"
            <h2>¡Bienvenido al Equipo, {name}! 🎉</h2>
            <p>Se ha creado exitosamente tu cuenta de usuario en <strong>ScoutAsset</strong>, la plataforma oficial para el control, trazabilidad e inventario del Grupo Scout Leonardo Murialdo N.° 1.</p>
            
            <div class='credential-card'>
                <div class='credential-header'>
                    🔐 Tus Credenciales de Acceso
                </div>
                <div class='credential-row'>
                    <span class='credential-label'>Usuario:</span>
                    <span class='credential-value'>{username}</span>
                </div>
                <div class='credential-row'>
                    <span class='credential-label'>Correo Registrado:</span>
                    <span class='credential-value'>{email}</span>
                </div>
                <div class='credential-row' style='margin-top: 8px;'>
                    <span class='credential-label'>Contraseña Temporal:</span>
                    <span class='password-badge'>{tempPassword}</span>
                </div>
            </div>

            <div class='security-alert'>
                <p><strong>🛡️ Medida de Seguridad Importante:</strong> Te recomendamos iniciar sesión de inmediato y actualizar tu contraseña desde el apartado de Perfil en la plataforma.</p>
            </div>

            <p>Para ingresar al sistema, haz clic en el siguiente botón:</p>
            <div class='button-wrapper'>
                <a href='https://localhost:49698/login' class='button'>Acceder a ScoutAsset</a>
            </div>";

        return BuildBaseTemplate($"Bienvenido a ScoutAsset, {name}", content);
    }

    public static string GetForgotPasswordEmail(string username, string resetLink)
    {
        var content = $@"
            <h2>Restablecimiento de Contraseña 🔑</h2>
            <p>Hola, <strong>{username}</strong>:</p>
            <p>Hemos recibido una solicitud para restablecer la contraseña asociada a tu cuenta en <strong>ScoutAsset</strong>.</p>
            <p>Haz clic en el botón a continuación para definir una nueva contraseña segura:</p>
            
            <div class='button-wrapper'>
                <a href='{resetLink}' class='button'>Restablecer Contraseña</a>
            </div>

            <div class='security-alert'>
                <p><strong>⚠️ Nota de Seguridad:</strong> Este enlace de recuperación expirará pronto. Si no solicitaste este cambio, puedes ignorar este correo de forma segura.</p>
            </div>";

        return BuildBaseTemplate("Restablecer Contraseña — ScoutAsset", content);
    }
}
