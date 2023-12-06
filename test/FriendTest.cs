﻿using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Dapper;
using FluentAssertions;
using FluentAssertions.Execution;
using Infastructure;
using Newtonsoft.Json;

namespace test;

[TestFixture]
public class FriendTest
{
    private string resetbd = $@"
DROP SCHEMA IF EXISTS keepsocial CASCADE;
CREATE SCHEMA keepsocial;
 
create table if not exists keepsocial.users (
    id integer generated by default as identity,
    name VARCHAR(100) NOT NULL UNIQUE,
    email VARCHAR(100) NOT NULL UNIQUE,
    profileDescription VARCHAR(500),
    birthday DATE,
    avatarUrl VARCHAR,
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

create table if not exists keepsocial.posts
(
    id integer generated by default as identity,
    author_id integer not null ,
    text VARCHAR(500) not null ,
    img_url VARCHAR,
    created timestamp not null,
    constraint postpk
        primary key (id),
    foreign key(author_id) references keepsocial.users(id)
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
    time_Send timestamp,

    FOREIGN KEY(rom_id) REFERENCES keepsocial.chatrooms(rom_id),
    FOREIGN KEY(user_id) REFERENCES keepsocial.users(id)
);

create table if not exists keepsocial.friendRealeatioj
(
    user1_id integer,
    user2_id integer,

    CONSTRAINT PK_friends PRIMARY KEY (user1_id, user2_id),
    FOREIGN KEY (user1_id) REFERENCES keepsocial.users (id),
    FOREIGN KEY (user2_id) REFERENCES keepsocial.users (id)
);

create table if not exists keepsocial.friendRequestTable
(
    request_id integer generated by default as identity,
    requester integer,
    requested integer,
    response bool,

    CONSTRAINT PK_friendRequest PRIMARY KEY (request_id),
    FOREIGN KEY (requester) REFERENCES keepsocial.users (id),
    FOREIGN KEY (requested) REFERENCES keepsocial.users (id)
);

create table if not exists keepsocial.validationnumbers
(
  user_id integer NOT NULL,
  validation_number integer NOT NULL,
  FOREIGN KEY (user_id) REFERENCES keepsocial.users(id)
);

create table if not exists keepsocial.comments
(
   id integer generated by default as identity,
   post_id integer not null,
   author_id integer not null,
   text VARCHAR(500) not null,
   img_url varchar,
   created timestamp not null,
   primary key (id),
   foreign key(post_id) references keepsocial.posts(id),
   foreign key (author_id) references keepsocial.users(id)
);

create table if not exists keepsocial.followrelation
(
    followed_id INT NOT NULL,
    follower_id INT NOT NULL,
    CONSTRAINT PK_follow PRIMARY KEY (followed_id, follower_id),
    FOREIGN KEY (followed_id) REFERENCES keepsocial.users(id),
    FOREIGN KEY (follower_id) REFERENCES keepsocial.users(id)
);

insert into keepsocial.users(id,name,email,birthday,isdeleted) values (111,'test tester','test@email.com',date('2023-02-24'),false);
insert into keepsocial.password_hash(user_id,hash,salt,algorithm) values (111,'6Po8CEcjkfC5Scoze2n5WoI7yLFePCZlEmQNPY9M0UFP0ghMegnM2t3k8HpcE/EkdYxOPJDj1XBM9J47eOdON6N9thahuXvlbY3D8Ag/y1JgHNw2ea6E5l1VWVSz7kCrQudnazfVTMQuce4emRjSLSAZAaoF55zXmBYjFOqZ5xX13TV5/UJUPkKf7uRKGNyTf1o/LlnZqmMsziF6unmCZTBgpn3W1oHr3zSh2Xm7dohmVN8+eDXPchVhUTha4u7QG75EFsILQfgALPDt3N/DUX/pHKBTVGcMAQWX1zAKyXdNPsp3MtiioBFnsJ+Zpn37mTRWNAPUMojvFekGGvb9zQ==','WwoEFhsDKJMTyb4GzbfV8inzImVI404JZSeQXWoptlUaZmuoTrKq5b/5WIqIXpeDuny7WbgDf3Hi3SuK1EmPNqTxGNbSJk3bPxufdhxwlaPXHa4vPOajEqdtLDqD+WpjvF5QK/oSTS/XB8Rj0oY0BOqW/k+KBiMTjyNc2fjJEpQ=','argon2id');
insert into keepsocial.users(id,name,email,birthday,isdeleted) values (112,'test','secondtestUser@email.com',date('2023-02-10'),false);
insert into keepsocial.password_hash(user_id,hash,salt,algorithm) values (112,'UhmSA3bJX5Owp8bm89NwO3V7VwSopx5vfBEFEAHnLKDSYzYxf7s/bNGG6VXLukD3N/hn+yUNJjnn120rrg7THpGf/8SzzrrSeMYWZ9xYA0bOfgR+lFl4zcrfy+vlgnudg1aHYVaj52VBodirzEg+cwg1JaXf51Rl8DRd+NFLO9OA1avvbkKwx+Ww2PX1nACHUqzKd/WzvSJLNYaRizLj95hRqRlRj44HyrID4unMCLIW87NN0j9UJ0Firo9u7xje1gZCz419IA1SABPRZcW+Gr6OIGPZeG2riTqluJuJgANyeIBPKAw7MXSO8Vz6THER5Jzbvl4Rwz/pMQ1eblmbJQ==','x4eVwCCfnM6132etvWCFHZV8ZN55E0h4BoR39AJRMeGoZ7xXUToBWjZoFSb9d1Ilu0wx3TN52dvh5qNo0cdodCaXREIWluHOv+ia+jfldvhTI9BQEsNKmEQSnV/TaAeBV1Kx+9GMICb0ZiZDxUTYMfjiHRByGGXK+dfRvoOIgcA=','argon2id');

";

    private string apirUrl = "";
    private HttpClient _httpClient;

    [SetUp]
    public void Setup()
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", Environment.GetEnvironmentVariable("testToken"));
        apirUrl = "http://localhost:5000/api/";
    }

    public class RequestUpdateDto
    {
        public required int RequesterId { get; set; }

        public required int RequestId { get; set; }

        public required bool Response { get; set; }
    }

    [Test]
    public async Task AcceptRequest()
    {
        string apicall = apirUrl + "account/FriendRequestsResponse";
        Helper.TriggerRebuild(resetbd);

        var sqlSetFriendrequest =
            $@"INSERT INTO keepsocial.friendRequestTable(request_id, requester, requested) VALUES (101, 112, 111);";
        using (var conn = Helper.DataSource.OpenConnection())
        {
            conn.Query(sqlSetFriendrequest);
        }

        var body = new RequestUpdateDto()
        {
            RequesterId = 112,
            RequestId = 101,
            Response = true
        };

        HttpResponseMessage responseMessage;
        try
        {
            responseMessage = await _httpClient.PutAsJsonAsync(apicall, body);
            TestContext.WriteLine("full body response: " + await responseMessage.Content.ReadAsStringAsync());
        }
        catch (Exception e)
        {
            throw new Exception("Failed to create resposne to request", e);
        }

        var sql = @$"SELECT count(user1_id) from keepsocial.friendRealeatioj 
            where (user1_id = 111 and user2_id = 112) 
        OR (user1_id = 112 and user2_id = 111);";
        
        var sql2 = @$"SELECT count(*) from keepsocial.friendRequestTable 
            WHERE requested = 111 and requester = 112";

        bool isFriends = false;
        bool requestexist = false;
        using (var conn = Helper.DataSource.OpenConnection())
        {
            isFriends = conn.ExecuteScalar<int>(sql) >= 1;
            requestexist = conn.ExecuteScalar<int>(sql2) >= 1;
        }

        using (new AssertionScope())
        {
            responseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

            isFriends.Should().Be(true);
            requestexist.Should().Be(false);
        }
    }

    [Test]
    public async Task DeclineRequest()
    {
        string apicall = apirUrl + "account/FriendRequestsResponse";
        Helper.TriggerRebuild(resetbd);

        var sqlSetFriendrequest =
            $@"INSERT INTO keepsocial.friendRequestTable(request_id, requester, requested) VALUES (101, 112, 111);";
        using (var conn = Helper.DataSource.OpenConnection())
        {
            conn.Query(sqlSetFriendrequest);
        }

        var body = new RequestUpdateDto()
        {
            RequesterId = 112,
            RequestId = 101,
            Response = false
        };

        HttpResponseMessage responseMessage;
        try
        {
            responseMessage = await _httpClient.PutAsJsonAsync(apicall, body);
            TestContext.WriteLine("full body response: " + await responseMessage.Content.ReadAsStringAsync());
        }
        catch (Exception e)
        {
            throw new Exception("Failed to create resposne to request", e);
        }

        var sql = $@"select request_id as {nameof(RequestUpdateDto.RequestId)},
       requester as {nameof(RequestUpdateDto.RequesterId)},
       response as {nameof(RequestUpdateDto.Response)}
        from keepsocial.friendRequestTable where requested = 111";

        RequestUpdateDto Checkrequestanwser = null;
        using (var conn = Helper.DataSource.OpenConnection())
        {
            Checkrequestanwser = conn.QuerySingle<RequestUpdateDto>(sql);
        }

        using (new AssertionScope())
        {
            responseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

            body.RequesterId.Should().Be(Checkrequestanwser.RequesterId);
            body.RequestId.Should().Be(Checkrequestanwser.RequestId);
            body.Response.Should().Be(Checkrequestanwser.Response);
        }
    }

    [Test]
    public async Task SendRequest()
    {
        
    }
}