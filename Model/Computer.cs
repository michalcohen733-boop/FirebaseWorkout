namespace FirebaseWorkout.Model
{
    // מחלקת מחשב - מייצגת מחשב פיזי בבית הספר
    // נשמרת ב-Firebase תחת הנתיב computers/
    public class Computer
    {
        // מזהה ייחודי של המחשב ב-Firebase
        public string Id { get; set; } = string.Empty;
        // מספר המחשב (לזיהוי על ידי המשתמש)
        public string ComputerNumber { get; set; } = string.Empty;
        // שם החדר/כיתה בה נמצא המחשב
        public string Room { get; set; } = string.Empty;
        // קוד QR ייחודי שמודבק על המחשב
        public string QRCode { get; set; } = string.Empty;
        // האם המחשב פעיל ובשימוש
        public bool IsActive { get; set; } = true;
    }
}
