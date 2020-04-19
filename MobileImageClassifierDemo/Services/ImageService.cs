using System;
using System.Threading.Tasks;

using Plugin.Media;
using Plugin.Media.Abstractions;

namespace MobileImageClassifierDemo.Services
{
    public static class ImageService
    {
        public static async Task<MediaFile> TakePhoto(bool useCamera)
        {
            await CrossMedia.Current.Initialize();
            MediaFile picture = null;

            try
            {
                if (useCamera)
                {
                    if (CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported)
                    {
                        picture = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                        {
                            SaveToAlbum = true
                        });
                    }
                }
                else
                    picture = await CrossMedia.Current.PickPhotoAsync();
            }
            catch (Exception ex)
            {

            }

            return picture;
        }
    }
}
