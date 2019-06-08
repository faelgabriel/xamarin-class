﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using Flurl;
using Flurl.Http;
using Xamarin.Forms;

namespace MultPlatProject
{

    public class BooksViewModel :
        // Essa interface indica que instâncias dessa classe disparam evento
        // avisando quando alguma propriedade muda de valor. Isso é usado pelos
        // bindings, para atualizar valores diversos
        INotifyPropertyChanged
    {
        bool mIsLoading;
        public bool IsLoading
        {
            get => mIsLoading;
            set
            {
                mIsLoading = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsLoading"));
            }
        }

        // Evento de propriedade alterada, deve ser disparado toda vez que uma
        // propriedade muda de valor. Definido em INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        // Evento disparado quando requisição falha.
        public event EventHandler<ErrorResponse> RequestFailed;

        // Propriedade de lista de livros consultados pelo ViewModel
        IEnumerable<Book> mBooks;
        public IEnumerable<Book> Books
        {
            get => mBooks; // No getter, meramente retorna variável interna
            set
            {
                // No setter, primeiro verifica se valor sendo atribuido é
                // diferente do atual
                if (!Equals(mBooks, value))
                {
                    // Se for, atribui novo valor a variável interna.
                    mBooks = value;

                    // E dispara evento indicando que a propriedade Books mudou.
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Books"));
                }
            }
        }

        // Commando de consulta de livros. Em tese, será executado quando usuário
        // tocar no botão de atualizar, ou quando for consultar os livros ao abrir
        // a tela.
        public ICommand GetCommand { get; private set; }

        public BooksViewModel()
        {
            // Função para ser executada quando o GetCommand for executado.
            async void execute()
            {
                try
                {
                    IsLoading = true;

                    // Vai atualizar livros
                    Books = await

                        // Começa pegando url do web service, como string
                        Constants.BaseServiceUrl

                                 // Namespace Flurl adiciona extensão em string
                                 // para concatenar mais um pedaço de rota a URL.
                                 // No caso, o pedaço da rota é o nome da classe
                                 // Book, para consultar os livros.
                                 .AppendPathSegment(typeof(Book).Name)

                                 .SetQueryParam("where", "title like '%" + TitleEntry + "%'")

                                 // Namespace Flurl também adiciona extensão em 
                                 // Url para realizar requisição HTTP GET na URL
                                 // especificada e, daí, desserializar o retorno
                                 // como um tipo passado (no caso, uma List de Book.
                                 .GetJsonAsync<List<Book>>();
                }
                // Se webservice retornar erro, lança uma FlurlHttpException
                catch (FlurlHttpException ex)
                {
                    // Nesse caso, pega o corpo da resposta de erro e
                    // desserializa como um DTO ErrorResponse
                    var error = await ex.GetResponseJsonAsync<ErrorResponse>();

                    // Invoca evento de requisição zoada.
                    RequestFailed?.Invoke(this, error);
                }
                finally
                {
                    IsLoading = false;
                }
            }

            // Atribui valor ao comando.
            GetCommand = new Command(execute);
        }

        public string TitleEntry { get; set; }
    }
}
