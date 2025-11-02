namespace SGBL.Application.Services
{
    public class EmailTemplateService
    {
        public static string CreateEmailConfirmationTemplate(string userName, string confirmationLink)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: #007bff; color: white; padding: 20px; text-align: center; }}
        .content {{ background: #f9f9f9; padding: 30px; }}
        .button {{ background: #007bff; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; display: inline-block; }}
        .footer {{ text-align: center; margin-top: 20px; font-size: 12px; color: #666; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Bienvenido a SGBL</h1>
        </div>
        <div class='content'>
            <h2>Hola {userName},</h2>
            <p>¡Estás a un paso de activar tu cuenta en el Sistema SGBL!</p>
            <p>Para completar tu registro, por favor confirma tu dirección de email haciendo clic en el siguiente botón:</p>
            <p style='text-align: center;'>
                <a href='{confirmationLink}' class='button'>Confirmar mi cuenta</a>
            </p>
            <p>Si el botón no funciona, copia y pega este enlace en tu navegador:</p>
            <p style='word-break: break-all;'>{confirmationLink}</p>
            <p><strong>Este enlace expirará en 24 horas.</strong></p>
        </div>
        <div class='footer'>
            <p>Si no solicitaste este registro, por favor ignora este mensaje.</p>
            <p>© {DateTime.Now.Year} SGBL Sistema. Todos los derechos reservados.</p>
        </div>
    </div>
</body>
</html>";
        }

        public static string CreatePasswordResetTemplate(string userName, string resetLink)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: #dc3545; color: white; padding: 20px; text-align: center; }}
        .content {{ background: #f9f9f9; padding: 30px; }}
        .button {{ background: #dc3545; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; display: inline-block; }}
        .footer {{ text-align: center; margin-top: 20px; font-size: 12px; color: #666; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Restablecer contraseña - SGBL</h1>
        </div>
        <div class='content'>
            <h2>Hola {userName},</h2>
            <p>Recibimos una solicitud para restablecer tu contraseña en el sistema SGBL.</p>
            <p>Haz clic en el siguiente botón para crear una nueva contraseña:</p>
            <p style='text-align: center;'>
                <a href='{resetLink}' class='button'>Restablecer contraseña</a>
            </p>
            <p>Si no solicitaste este cambio, por favor ignora este mensaje.</p>
            <p><strong>Este enlace expirará en 1 hora.</strong></p>
        </div>
        <div class='footer'>
            <p>© {DateTime.Now.Year} SGBL Sistema.</p>
        </div>
    </div>
</body>
</html>";
        }
    }
}