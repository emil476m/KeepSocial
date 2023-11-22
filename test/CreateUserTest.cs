using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using FluentAssertions.Execution;
using Newtonsoft.Json;
using Npgsql;

namespace test;
public class ResponseDto
{ 
    public string MessageToClient { get; set; } 
    public object? ResponseData { get; set; }
    
}


public class testUserObject
{
    public string userDisplayName { get; set; }
    public string userEmail { get; set; }
    public DateTime userBirthday { get; set; }
    public string password { get; set; }
    public string? AvatarUrl { get; set; }
}

public class CreateUserTest
{
    private string apirUrl = "";
    private HttpClient _httpClient;
    private readonly NpgsqlDataSource _dataSource;

    private string resetbd = $@"
DROP SCHEMA IF EXISTS keepsocial CASCADE;
CREATE SCHEMA keepsocial;

create table if not exists keepsocial.users (
    id integer generated by default as identity,
    name VARCHAR(100) NOT NULL,
    email VARCHAR(100) NOT NULL,
    birthday DATE,
    isDeleted boolean NOT NULL,
    constraint userpk
        primary key (id)
);

create table if not exists keepsocial.password_hash (
    user_id integer,
    hash VARCHAR(350) NOT NULL ,
    salt VARCHAR(180) NOT NULL ,
    algorithm VARCHAR(12) NOT NULL ,
    FOREIGN KEY(user_id) REFERENCES keepsocial.users(id)
);

create table if not exists keepsocial.chatrooms (
    rom_id integer generated by default as identity,
    rom_name VARCHAR,

    primary key (rom_id)
);

create table if not exists keepsocial.chatroomUsersRealation (
    rom_id integer ,
    user_id integer,

    FOREIGN KEY(rom_id) REFERENCES keepsocial.chatrooms(rom_id),
    FOREIGN KEY(user_id) REFERENCES keepsocial.users(id)
);

create table if not exists keepsocial.messages (
    rom_id integer,
    user_id integer,
    message VARCHAR,

    FOREIGN KEY(rom_id) REFERENCES keepsocial.chatrooms(rom_id),
    FOREIGN KEY(user_id) REFERENCES keepsocial.users(id)
);";
    
    [SetUp]
    public void Setup()
    {
        _httpClient = new HttpClient();
        apirUrl = "http://localhost:5000/api/account/createuser";
    }

    
    
    [Test]
    public async Task ShouldSuccessfullyCreateUser()
    {
        Helper.TriggerRebuild(resetbd);
        var user = new testUserObject()
        {
            userDisplayName = "testingcreateName",
            userEmail = "testingmail@gmail.com",
            userBirthday = new DateTime(2000, 1, 1),
            
            password = "12345678",
    
            AvatarUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/2/2c/Default_pfp.svg/2048px-Default_pfp.svg.png",
        };
        
        ResponseDto expectedResponseDTO = new ResponseDto
        {
            MessageToClient = "Successfully registered"
        };
        
        

        var url = "http://localhost:5000/api/account/createuser";
        
        HttpResponseMessage response;
        
        try
        {
            response = await _httpClient.PostAsJsonAsync(url, user);
            TestContext.WriteLine("THE FULL BODY RESPONSE: " + await response.Content.ReadAsStringAsync());
        }
        catch (Exception e)
        {
            throw new Exception(Helper.NoResponseMessage, e);
        }

        ResponseDto responseObj;
        try
        {
            responseObj = JsonConvert.DeserializeObject<ResponseDto>(await response.Content.ReadAsStringAsync()) ??
                          throw new InvalidOperationException();
        }
        catch (Exception e)
        {
            throw new Exception(Helper.BadResponseBody(await response.Content.ReadAsStringAsync()), e);
        }
        
        
        
        using (new AssertionScope())
        {
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseObj.MessageToClient.Should().Be(expectedResponseDTO.MessageToClient);
        }
    }

    /*private static DateTime testcase1Date = new DateTime(1999, 2, 2);
    [TestCase("testingName1", "testingmail123@gmail.com", testcase1Date , "12345678", "url")]
    public async Task ShouldFailDueToDataValidation(string name, string email, DateTime birthday, string password, string avatarUrl)
    {
        var user = new testUserObject()
        {
            userId =-101,
            userDisplayName = "testingcreateName",
            userEmail = "testingmail@gmail.com",
            userBirthday = new DateTime(2000, 1, 1),
    
            AvatarUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/2/2c/Default_pfp.svg/2048px-Default_pfp.svg.png",
    
            isDeleted = false,
        };

        HttpResponseMessage response;
        try
        {
            response = await _httpClient.PostAsJsonAsync("http://localhost:5000/api/account/createuser", user);
            TestContext.WriteLine("THE FULL BODY RESPONSE: " + await response.Content.ReadAsStringAsync());
        }
        catch (Exception e)
        {
            throw new Exception(Helper.NoResponseMessage, e);
        }
        
        response.IsSuccessStatusCode.Should().BeFalse();
    }*/
}