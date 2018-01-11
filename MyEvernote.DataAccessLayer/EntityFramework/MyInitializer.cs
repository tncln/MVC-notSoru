using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using MyEvernote.Entities;

namespace MyEvernote.DataAccessLayer.EntityFramework
{
    public class MyInitializer : CreateDatabaseIfNotExists<DatabaseContext>
    {
        protected override void Seed(DatabaseContext context)
        {
            //admin user ekleme
            EvernoteUser admin = new EvernoteUser()
            {
                Name = "Adem",
                Surname = "Tunçalın",
                Email = "tuncalin@outlook.com",
                ActivateGuid = Guid.NewGuid(),
                IsActive = true,
                IsAdmin = true,
                Username = "ademtunc",
                ProfileImageFilename = "user_profile.gif",
                Password = "2465685",
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now.AddMinutes(5),
                ModifiedUsername = "ademtunc"

            };
            //standart user ekleme 
            EvernoteUser standartUser = new EvernoteUser()
            {
                Name = "user",
                Surname = "Tunçalın",
                Email = "admin@outlook.com",
                ActivateGuid = Guid.NewGuid(),
                IsActive = true,
                IsAdmin = false,
                Username = "user",
                ProfileImageFilename = "user_profile.gif",
                Password = "1234567",
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now.AddMinutes(5),
                ModifiedUsername = "ademtunc"
            };
            context.EvernoteUsers.Add(admin);
            context.EvernoteUsers.Add(standartUser);

            for (int i = 0; i < 8; i++)
            {
                EvernoteUser user = new EvernoteUser()
                {
                    Name = FakeData.NameData.GetFirstName(),
                    Surname = FakeData.NameData.GetSurname(),
                    Email = FakeData.NetworkData.GetEmail(),
                    ActivateGuid = Guid.NewGuid(),
                    IsActive = true,
                    IsAdmin = false,
                    Username = $"user{i}",
                    ProfileImageFilename = "user_profile.gif",
                    Password = "123",
                    CreatedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                    ModifiedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                    ModifiedUsername = $"user{i}"   
                };
                context.EvernoteUsers.Add(user);
            }

            context.SaveChanges();
            //user listesi
            List<EvernoteUser> userlist = context.EvernoteUsers.ToList();
            //Fake Categories
            for (int i = 0; i < 10; i++)
            {
                Category cat = new Category()
                {
                    Title = FakeData.PlaceData.GetStreetName(),
                    Description = FakeData.PlaceData.GetAddress(),
                    
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    ModifiedUsername = "ademtunc",

                };
                context.Categories.Add(cat);
                //fake note ekleme   
                for (int k = 0; k < FakeData.NumberData.GetNumber(5, 9); k++)
                {
                    EvernoteUser owner = userlist[FakeData.NumberData.GetNumber(0, userlist.Count - 1)];
                    Note note = new Note()
                    {
                        Title = FakeData.TextData.GetAlphabetical(FakeData.NumberData.GetNumber(5, 25)),
                        Text = FakeData.TextData.GetSentences(FakeData.NumberData.GetNumber(5, 25)),
                        IsDraft = false,
                        LikeCount = FakeData.NumberData.GetNumber(1, 9),
                        Owner = owner,
                        CreatedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                        ModifiedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                        ModifiedUsername = owner.Username,

                    };
                    cat.Notes.Add(note);

                    //Fake comments ekleme
                    for (int j = 0; j < FakeData.NumberData.GetNumber(3, 5); j++)
                    {
                        EvernoteUser comment_owner = userlist[FakeData.NumberData.GetNumber(0, userlist.Count - 1)];
                        Comment comment = new Comment()
                        {
                            Text = FakeData.TextData.GetSentence(),
                            Owner = comment_owner,
                            CreatedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                            ModifiedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                            ModifiedUsername = comment_owner.Username
                        };
                        note.Comments.Add(comment);
                    }
                    //Fake like ekleme..
                    

                    for (int m = 0; m < note.LikeCount; m++)
                    {
                        Liked liked = new Liked()
                        {
                            LikedUser = userlist[m]
                        };
                        note.Likes.Add(liked);
                    }
                }
            }
            context.SaveChanges();
        }
    }
}
