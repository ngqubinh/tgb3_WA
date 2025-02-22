namespace Application.ViewModels.User
{
    public class EditProfileVM
    {
        public string? FullName { get; set; }       
        public string? PhoneNumber { get; set; }
        public DateOnly? BirthDate { get; set; }
        public string? Sex { get; set; }
        public ProfileVM? Profile { get; set; }
        public string? BirthDateDay { get; set; }
        public string? BirthDateMonth { get; set; }
        public string? BirthDateYear { get; set; }
        public void CombineBirthDate()
        {
            if(!string.IsNullOrEmpty(this.BirthDateDay) && !string.IsNullOrEmpty(this.BirthDateMonth) && !string.IsNullOrEmpty(this.BirthDateYear))
            {
                int day = int.Parse(this.BirthDateDay);
                int month = int.Parse(this.BirthDateMonth);
                int year = int.Parse(this.BirthDateYear);
                BirthDate = new DateOnly(year, month, day);
            }
        }
        public void SplitBirthDate()
        {
            if(BirthDate.HasValue)
            {
                this.BirthDateDay = this.BirthDate.Value.Day.ToString("00");
                this.BirthDateMonth = this.BirthDate.Value.Month.ToString("00");
                this.BirthDateYear = this.BirthDate.Value.Year.ToString();
            }
        }
    }
}