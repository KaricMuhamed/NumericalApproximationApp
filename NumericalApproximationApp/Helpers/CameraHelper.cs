using NumericalApproximationApp.Services;

namespace NumericalApproximationApp.Helpers
{
    public static class CameraHelper
    {
        public static async Task<string> ScanFunctionAsync(OCRService ocrService, Page page)
        {
            string action = await page.DisplayActionSheet(
                "Skeniranje funkcije",
                "Otkaži",
                null,
                "📷 Slikaj kamerom",
                "🖼️ Odaberi iz galerije");

            byte[] imageData = null;

            if (action == "📷 Slikaj kamerom")
            {
                imageData = await ocrService.CapturePhotoAsync();
            }
            else if (action == "🖼️ Odaberi iz galerije")
            {
                imageData = await ocrService.PickPhotoAsync();
            }

            if (imageData != null)
            {
                string recognized = await ocrService.RecognizeFromImageAsync(imageData);

                if (!string.IsNullOrEmpty(recognized))
                {
                    // Prikaži korisniku prepoznati tekst i dozvoli izmjenu
                    string confirmed = await page.DisplayPromptAsync(
                        "Prepoznata funkcija",
                        "Provjerite i ispravite ako treba:",
                        "OK",
                        "Otkaži",
                        initialValue: recognized);

                    return confirmed;
                }
                else
                {
                    await page.DisplayAlert("Greška", "Nije moguće prepoznati tekst sa slike.", "OK");
                }
            }

            return null;
        }
    }
}