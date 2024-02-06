using System.Text.RegularExpressions;
using DeliveryAPI.DbContext;
using DeliveryAPI.Exceptions;
using DeliveryAPI.Models;
using DeliveryAPI.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace DeliveryAPI.Helpers;

public abstract class CheckInfoHelper
{
    public static string NormilizeEmail(string email)
    {
        return email.ToLower().TrimEnd();
    }

    public static void CheckGender(Gender gender)
    {
        if(gender is Gender.Male or Gender.Female)
        {
           return;
        }
        throw new BadRequest("Invalid gender");
    }

    public static void CheckValidDate(DateTime? dateTime)
    {
        if (dateTime == null) return;
        if (dateTime > DateTime.Now)
        {
            throw new BadRequest("Invalid age");
        }

        int age = DateTime.UtcNow.Year - dateTime.Value.Year;
        if (DateTime.UtcNow.Month < dateTime.Value.Month ||
            (DateTime.UtcNow.Month == dateTime.Value.Month
             && DateTime.UtcNow.Day < dateTime.Value.Day)) age--;
        if (age is < 6 or > 100)
        {
            throw new BadRequest("Invalid age");
        }

    }

    public static void CheckPhone(string? phoneNumber)
    {
        var phoneRegex = new Regex(@"^((8|\+7)[\- ]?)?(\(?\d{3}\)?[\- ]?)?[\d\- ]{7,10}$");
        var matches = phoneRegex.Match(phoneNumber);
        if (matches.Length <= 0)
        {
            throw new BadRequest("Invalid phone number");
        }
    }

    public static async Task CheckAddress(Guid? id, DeliveryContext deliveryContext)
    {

        var house = await deliveryContext.AsHouses.FirstOrDefaultAsync(x => x.Objectguid == id);
        if (house != null)
        {
            return;
        }

        var obj = await deliveryContext.AsAddrObjs.FirstOrDefaultAsync(x => x.Objectguid == id);
        if (obj != null)
        {
            throw new BadRequest("Address must be a house");
        }

        throw new NotFoundException("Address not found");
    }
    public static void CheckDeliveryTime(DateTime deliveryTime)
    {
        DateTime currentUtcTime = DateTime.UtcNow;

        if (deliveryTime <= currentUtcTime)
        {
            throw new BadRequest("Delivery time must be in the future");
        }

        TimeSpan timeDifference = deliveryTime - currentUtcTime;

        if (timeDifference.TotalHours < 1)
        {
            throw new BadRequest("Delivery must be at least one hour in the future");
        }
    }


    public static void CheckRating(int ratingScore)
    {
        if (ratingScore < 0 || ratingScore > 5) throw new BadRequest("Rating score must be from 0 to 5");
    }
}
