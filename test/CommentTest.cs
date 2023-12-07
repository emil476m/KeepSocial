﻿using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using FluentAssertions.Execution;
using Infastructure;
using Newtonsoft.Json;

namespace test;

[TestFixture]
public class CommentTest
{

    private string resetDb = $@"DROP SCHEMA IF EXISTS keepsocial CASCADE;
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
insert into keepsocial.posts(id,author_id,text,img_url,created) values (1,111,'yes a test10','qwderqdwefrfwf',date('2023-02-28'));";
    
    private string apirUrl = "";
    private HttpClient _httpClient;

    [SetUp]
    public void Setup()
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", Environment.GetEnvironmentVariable("testToken"));
        apirUrl = "http://localhost:5000/api/comment/?postId=1";
    }

    [Test]
    public async Task creatComment()
    {
        Helper.TriggerRebuild(resetDb);

        var comment = new CommentDto
        {
            authorId = 0,
            text = "hello there good tester",
            imgurl = "no",
        };
        
        HttpResponseMessage responseMessage;
        try
        {
            responseMessage = await _httpClient.PostAsJsonAsync(apirUrl, comment);
            TestContext.WriteLine("full body response: " + await responseMessage.Content.ReadAsStringAsync());
        }
        catch (Exception e)
        {
            throw new Exception("Failed to create the post", e);
        }

        Comment response;
        try
        {
            response = JsonConvert.DeserializeObject<Comment>(await responseMessage.Content.ReadAsStringAsync()) ??
                       throw new InvalidOperationException();
        }
        catch (Exception e)
        {
            throw new Exception(Helper.BadResponseBody("bad response"), e);
        }
        using (new AssertionScope())
        {
            responseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
            response.authorId.Should().NotBe(comment.authorId);
            response.id.Should().NotBe(0);
            response.text.Should().Be(comment.text);
            response.imgUrl.Should().Be(comment.imgurl);
        }
    }


    [Test]
    public async Task getComments()
    {
        Helper.TriggerRebuild(resetDb + "insert into keepsocial.comments(post_id,author_id,text,img_url,created) values (1,111,'yes','no',date ('2023-11-11'));");
        apirUrl = "http://localhost:5000/api/getcomments?postId=1";
        
        HttpResponseMessage responseMessage;
        try
        {
            responseMessage = await _httpClient.GetAsync(apirUrl);
            TestContext.WriteLine("full body response: " + await responseMessage.Content.ReadAsStringAsync());
        }
        catch (Exception e)
        {
            throw new Exception("Failed to get comments", e);
        }
        
        List<Comment> result;
        try
        {
            result = JsonConvert.DeserializeObject<List<Comment>>(await responseMessage.Content.ReadAsStringAsync()) ??
                     throw new InvalidOperationException();
        }
        catch (Exception e)
        {
            throw new Exception(Helper.BadResponseBody("bad response"), e);
        }
        
        
        using (new AssertionScope())
        {
            responseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Count.Should().NotBe(null);
            result[0].id.Should().NotBe(0);
        }
    }

    [Test]
    public async Task getMoreComents()
    {
        apirUrl = "http://localhost:5000/api/getmorecomments?limit=10&offset=0&postId=1";
        Helper.TriggerRebuild(resetDb + "insert into keepsocial.comments(post_id,author_id,text,img_url,created) values (1,111,'yes','no',date ('2023-11-11'));");
        
        HttpResponseMessage responseMessage;
        try
        {
            responseMessage = await _httpClient.GetAsync(apirUrl);
            TestContext.WriteLine("full body response: " + await responseMessage.Content.ReadAsStringAsync());
        }
        catch (Exception e)
        {
            throw new Exception("Failed to get more comments", e);
        }

        List<Comment> result;
        try
        {
            result = JsonConvert.DeserializeObject<List<Comment>>(await responseMessage.Content.ReadAsStringAsync()) ??
                       throw new InvalidOperationException();
        }
        catch (Exception e)
        {
            throw new Exception(Helper.BadResponseBody("bad response"), e);
        }
        
        
        using (new AssertionScope())
        {
            responseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Count.Should().NotBe(null);
            result[0].id.Should().NotBe(0);
        }
    }

    [Test]
    public async Task updateComment()
    {
        apirUrl = "http://localhost:5000/api/comment/1";
        Helper.TriggerRebuild(resetDb + "insert into keepsocial.comments(post_id,author_id,text,img_url,created) values (1,111,'yes','no',date ('2023-11-11'));");

        
        var comment = new CommentDto
        {
            authorId = 0,
            postId = 1,
            text = "hello there good tester",
            imgurl = "yes",
        };
        
        HttpResponseMessage responseMessage;
        try
        {
            responseMessage = await _httpClient.PutAsJsonAsync(apirUrl, comment);
            TestContext.WriteLine("full body response: " + await responseMessage.Content.ReadAsStringAsync());
        }
        catch (Exception e)
        {
            throw new Exception("Failed to update the post", e);
        }

        Comment response;
        try
        {
            response = JsonConvert.DeserializeObject<Comment>(await responseMessage.Content.ReadAsStringAsync()) ??
                       throw new InvalidOperationException();
        }
        catch (Exception e)
        {
            throw new Exception(Helper.BadResponseBody("bad response"), e);
        }
        using (new AssertionScope())
        {
            responseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
            response.postId.Should().Be(comment.postId);
            response.authorId.Should().NotBe(comment.authorId);
            response.id.Should().NotBe(0);
            response.text.Should().Be(comment.text);
            response.imgUrl.Should().Be(comment.imgurl);
        }
    }

    [Test]
    public async Task deleteComment()
    {
        apirUrl = "http://localhost:5000/api/deletecomment?id=1";
        Helper.TriggerRebuild(resetDb + "insert into keepsocial.comments(post_id,author_id,text,img_url,created) values (1,111,'yes','no',date ('2023-11-11'));");
        var sqlcheck = $@"SELECT COUNT(*) FROM keepsocial.comments;";
        
        
        HttpResponseMessage responseMessage;
        try
        {
            responseMessage = await _httpClient.DeleteAsync(apirUrl);
            TestContext.WriteLine("full body response: " + await responseMessage.Content.ReadAsStringAsync());
        }
        catch (Exception e)
        {
            throw new Exception("Failed to update the post", e);
        }

        var dbcheck = Helper.checkifexists(sqlcheck);
        using (new AssertionScope())
        {
            responseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
            dbcheck.Should().Be(0);
        }
    }
}


public class CommentDto
{
    public int postId { get; set; }
    public int authorId { get; set; }
    [MinLength(3)]
    [MaxLength(500)]
    public string text { get; set; }
    public string? imgurl { get; set; }
    public DateTimeOffset? created { get; set; }
}