using UnityEngine;
using System.Collections;
using System.Globalization;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class Validate : MonoBehaviour {

	static Regex Valid_Name = ValidUserName();              //USer_Name
	static Regex Valid_Password = ValidPassword();     		//Password
	static Regex Valid_Email = ValidEmail_Address();        //Email_Adress
	static Regex Valid_Url = Valid_Url_Address();
	public enum ErrorCode
	{
		VALID,
		INVALID
	}

	/// <summary>
	/// For Checking UserName
	/// </summary>
	/// <returns>The only.</returns>
	private static Regex ValidUserName()
	{
		string StringAndNumber_Pattern = "^[a-zA-Z]";
		return new Regex(StringAndNumber_Pattern, RegexOptions.IgnoreCase);
	}
	/// <summary>
	/// For Checking Email Address
	/// </summary>
	/// <returns>The address.</returns>
	private static Regex ValidEmail_Address()
	{
		string Email_Pattern = /*@"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
			+ @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
				+ @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$"*/
			@"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
			+ @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
				+ @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
				+ @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";	
		return new Regex(Email_Pattern, RegexOptions.IgnoreCase);
	}
	/// <summary>
	/// For Checking Password 
	/// </summary>
	/// <returns>The password.</returns>
	private static Regex ValidPassword()
	{
		string Password_Pattern = "^.*(?=.{6,})(?=.*\\d)(?=.*[a-z])(?=.*[!*@#$%^&+=]).*$"; /*"((?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[$&+,:;=?@#|'<>.-^*()%!]).{6,20})"; */
		return new Regex(Password_Pattern);
	}
	/// <summary>
	/// Valid_s the url_ address.
	/// </summary>
	/// <returns>The url_ address.</returns>
	private static Regex Valid_Url_Address(){
		string Url_Pattern = "^(https?|ftp|file)://[-a-zA-Z0-9+&@#/%?=~_|!:,.;]*[-a-zA-Z0-9+&@#/%=~_|]";
		return new Regex (Url_Pattern,RegexOptions.IgnoreCase);
	}
	/// <summary>
	/// Submit button check all the Email,Password,UserName
	/// </summary>
	public ErrorCode ValidateUserName(string userName){
		if (userName == null) {
			return ErrorCode.INVALID;
		}
		if (userName.Trim() == "") {
			return ErrorCode.INVALID;	
		}
		if (!Valid_Name.IsMatch (userName)) {
			return ErrorCode.INVALID;
		} 
		return ErrorCode.VALID;
	}
	public ErrorCode ValidateEmail(string email){
		if (email == null) {
			return ErrorCode.INVALID;
		}
		if (email.Trim() == "") {
			return ErrorCode.INVALID;
		}
		if (!Valid_Email.IsMatch (email)) {
			return ErrorCode.INVALID;
		} 
		return ErrorCode.VALID;
	}
	public ErrorCode ValidatePassword(string password){
		if (password == null) {
			return ErrorCode.INVALID;
		}
		if (password.Trim() == "") {
			return ErrorCode.INVALID;
		}
		if (!Valid_Password.IsMatch (password)) {
			return ErrorCode.INVALID;
		} 
		return ErrorCode.VALID;
	}
	public ErrorCode ValidateUrl(string url){
		if (url == null) {
			return ErrorCode.INVALID;
		}
		if (!Valid_Url.IsMatch (url)) {
			return ErrorCode.INVALID;
		} 
		return ErrorCode.VALID;
	}

}
