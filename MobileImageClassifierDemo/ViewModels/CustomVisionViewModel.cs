using System.Threading.Tasks;

using MobileImageClassifierDemo.Services;

using Xamarin.Forms;
using Plugin.Media.Abstractions;

namespace MobileImageClassifierDemo.ViewModels
{
    public class CustomVisionViewModel : BaseViewModel
    {
        public Command<bool> TakePhotoCommand { get; set; }
        public Command<bool> ClassifyPhotoCommand { get; set; }

        private MediaFile photo;

        public MediaFile Photo
        {
            get { return photo; }
            set { photo = value; OnPropertyChanged(); OnPropertyChanged("PhotoStream"); }
        }

        private string classificationResult;

        public string ClassificationResult
        {
            get { return classificationResult; }
            set { classificationResult = value; OnPropertyChanged(); }
        }

        private bool isBusy;

        public bool IsBusy
        {
            get { return isBusy; }
            set { isBusy = value; OnPropertyChanged(); }
        }

        public ImageSource PhotoStream
        {
            get => this.photo != null ? ImageSource.FromStream(photo.GetStreamWithImageRotatedForExternalStorage) : null;
        }

        public CustomVisionViewModel()
        {
            TakePhotoCommand = new Command<bool>(async (useCamera) => await TakePhoto(useCamera));
            ClassifyPhotoCommand = new Command<bool>(async (useLocal) => await ClassifyPhoto(useLocal));
        }

        private async Task TakePhoto(bool useCamera)
        {
            ClassificationResult = "---";
            Photo = await ImageService.TakePhoto(useCamera);
        }

        private async Task ClassifyPhoto(bool useLocal)
        {
            if (Photo != null)
            {
                IsBusy = true;
                ClassificationResult = "...";

                ClassificationResult = useLocal
                    ? await CustomVisionLocalService.ClassifyImage(photo)
                    : await CustomVisionAzureService.ClassifyImage(photo);

                IsBusy = false;
            }
            else
                ClassificationResult = "---No image---";
        }
    }
}
