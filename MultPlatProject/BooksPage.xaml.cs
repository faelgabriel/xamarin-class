using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace MultPlatProject
{
    public partial class BooksPage : ContentPage
    {
        public BooksPage()
        {
            InitializeComponent();
        }

        // Executado toda vez que a tela for exibida.
        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Pega o view model pelo cast do BindingContext e executa o GetCommand.
            // Isso deve popular a lista de livros inicialmente.
            (BindingContext as BooksViewModel).GetCommand.Execute(null);
        }

        // Executado quando a requisição de consulta dos livros falha.
        void Handle_RequestFailed(object sender, MultPlatProject.ErrorResponse e)
        {
            // Mostra alerta.
            DisplayAlert("Erro", e.Message, "ok");
        }
    }
}
