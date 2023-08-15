using LIN.Shared.Responses;

namespace LIN.UI.Views;

public partial class Singin : ContentPage
{


    /// <summary>
    /// Constructor
    /// </summary>
	public Singin()
    {
        InitializeComponent();
    }



    /// <summary>
    /// Evento cuando se escribe sobre los txts
    /// </summary>
    private void TxtChanged(object sender, TextChangedEventArgs e)
    {
        lbInfo.Hide();
    }



    /// <summary>
    /// Evento Ir a Login
    /// </summary>
    private void GoLoginEvent(object sender, EventArgs e)
    {
        new Login().Show();
        this.Close();
    }



    /// <summary>
    /// Evento Crear
    /// </summary>
    private async void BtnCrear(object sender, EventArgs e)
    {
        // Modo de carga
        EnableChargeMode();

        await Task.Delay(10);

        // Variables
        string user = txtUser.Text ?? "";
        string name = txtName.Text ?? "";
        string pass = txtPassword.Text ?? "";

        // Campos vacios
        if (string.IsNullOrEmpty(user.Trim()) || string.IsNullOrEmpty(name.Trim()) || string.IsNullOrEmpty(pass.Trim()))
        {
            DisableChargeMode();
            lbInfo.Text = "Por favor, aseg�rate de llenar todos los campos requeridos.";
            lbInfo.Show();
            return;
        }


        // Contrase�a Lenght
        if (pass.Length < 4)
        {
            DisableChargeMode();
            lbInfo.Text = "La contrase�a debe de tener minimo 4 digitos";
            lbInfo.Show();
            return;
        }


        // Model
        LIN.Shared.Models.UserDataModel modelo = new()
        {
            Nombre = name,
            Usuario = user,
            Contrase�a = pass,
            Perfil = await inpImg.GetBytes()
        };

        // Creacion
        var res = await LIN.Access.Controllers.User.CreateAsync(modelo);

        // Respuesta
        switch (res.Response)
        {

            case Responses.Success:
                break;

            case Responses.NotConnection:
                DisableChargeMode();
                lbInfo.Text = "Error conexion";
                lbInfo.IsVisible = true;
                return;

            case Responses.ExistAccount:
                DisableChargeMode();
                lbInfo.Text = $"Ya existe un usuario con el nombre '{user}'";
                lbInfo.IsVisible = true;
                return;

            default:
                DisableChargeMode();
                lbInfo.Text = "Error grave";
                lbInfo.IsVisible = true;
                return;
        }


        // Plataforma
        Platforms platform = MauiProgram.GetPlatform();

        // Inicio de sesion
        var (Sesion, Response) = await Access.Sesion.LoginWith(user, pass, platform);


        // Evaluacion
        Login form;
        switch (Response)
        {

            case Responses.Success:
                break;

            case Responses.NotExistAccount:
                form = new();
                form.Show();
                this.Close();
                return;

            case Responses.InvalidPassword:
                form = new();
                form.Show();
                this.Close();
                return;


            case Responses.NotConnection:
                DisableChargeMode();
                lbInfo.Text = "No hay conexion";
                lbInfo.Show();
                return;

            // Hubo un error grave
            default:
                form = new();
                form.Show();
                this.Close();
                return;

        }


        // Abre la nueva ventana
        App.Current!.MainPage = new AppShell();

        this.Close();
    }



    /// <summary>
    /// Modo de carga
    /// </summary>
    private void EnableChargeMode()
    {
        btnCrear.Hide();
        lbInfo.Hide();
        lbsCrear.Hide();
        indicador.Show();
    }



    /// <summary>
    /// Deshabilita el modo de carga
    /// </summary>
    private void DisableChargeMode()
    {
        btnCrear.Show();
        lbInfo.Hide();
        lbsCrear.Show();
        indicador.Hide();
    }


}