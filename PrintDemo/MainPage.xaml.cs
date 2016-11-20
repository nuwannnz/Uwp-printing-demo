using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Printing;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Printing;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PrintDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        PrintDocument _printDocument;
        PrintManager _printMan;
        IPrintDocumentSource _prindDocumentSource;

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _printDocument = new PrintDocument();
            _prindDocumentSource = _printDocument.DocumentSource;

            _printDocument.Paginate += _printDocument_Paginate;
            _printDocument.GetPreviewPage += _printDocument_GetPreviewPage;
            _printDocument.AddPages += _printDocument_AddPages;

            _printMan = PrintManager.GetForCurrentView();
            _printMan.PrintTaskRequested += _printMan_PrintTaskRequested;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if(_printDocument != null)
            {
                _printDocument.Paginate -= _printDocument_Paginate;
                _printDocument.GetPreviewPage -= _printDocument_GetPreviewPage;
                _printDocument.AddPages -= _printDocument_AddPages;
            }

            if(_printMan != null)
            {
                _printMan.PrintTaskRequested -= _printMan_PrintTaskRequested;
            }
        }

        private void _printMan_PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs args)
        {
            PrintTask printTask = null;
            printTask = args.Request.CreatePrintTask("C# Printing SDK Sample", sourceRequested =>
            {
                // Print Task event handler is invoked when the print job is completed.
                printTask.Completed += async (s, e) =>
                {
                    // Notify the user when the print operation fails.
                    if (e.Completion == PrintTaskCompletion.Failed)
                    {
                        await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,async () =>
                        {
                            // Printing cannot proceed at this time
                            ContentDialog noPrintingDialog = new ContentDialog()
                            {
                                Title = "Printing error",
                                Content = "\nSorry, printing can' t proceed at this time.",
                                PrimaryButtonText = "OK"
                            };
                            await noPrintingDialog.ShowAsync();
                        });
                    }
                };

                sourceRequested.SetSource(_prindDocumentSource);
            });
        }

        private void _printDocument_AddPages(object sender, AddPagesEventArgs e)
        {
            _printDocument.AddPage(PrintGrid);
            _printDocument.AddPagesComplete();
        }

        private void _printDocument_GetPreviewPage(object sender, GetPreviewPageEventArgs e)
        {
            //set the page 
            _printDocument.SetPreviewPage(e.PageNumber, PrintGrid);
        }

        private void _printDocument_Paginate(object sender, PaginateEventArgs e)
        {
            _printDocument.SetPreviewPageCount(1, PreviewPageCountType.Final);
        }

        private async void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (Windows.Graphics.Printing.PrintManager.IsSupported())
            {
                try
                {
                    await PrintManager.ShowPrintUIAsync();
                }
                catch 
                {
                    // Printing cannot proceed at this time
                    ContentDialog noPrintingDialog = new ContentDialog()
                    {
                        Title = "Printing error",
                        Content = "\nSorry, printing can' t proceed at this time.",
                        PrimaryButtonText = "OK"
                    };
                    await noPrintingDialog.ShowAsync();
                }
            }else
            {
                // Printing cannot proceed at this time
                ContentDialog noPrintingDialog = new ContentDialog()
                {
                    Title = "Printing error",
                    Content = "\nSorry, printing is not supported in this device.",
                    PrimaryButtonText = "OK"
                };
                await noPrintingDialog.ShowAsync();
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
