/*
 * Copyright (C) 2023 IKTSolution
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
 * to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE 
 * OR OTHER DEALINGS IN THE SOFTWARE.
 */

using IdeaIncubatorBlazor.Models;

namespace IdeaIncubatorBlazor.Services.Ideas;

public class ChatService : IChatService
{
    private readonly IdeaIncubatorDbContext _dbContext;

    public ChatService(IdeaIncubatorDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Message CreateAMessage(Message message)
    {
        _dbContext.Messages.Add(message);
        _dbContext.SaveChanges();
        return message;
    }

    public Task<ChatGroup> CreateChatGroup(int ideaId)
    {
        ChatGroup chatGroup = new ChatGroup { IdeaId = ideaId };
        _dbContext.ChatGroups.Add(chatGroup);
        _dbContext.SaveChanges();
        return Task.FromResult(chatGroup);
    }

    public Task<List<ChatGroupMember>> CreateChatGroupMembers(List<ChatGroupMember> chatGroupMembers)
    {
        _dbContext.ChatGroupMembers.AddRange(chatGroupMembers);
        _dbContext.SaveChanges();
        return Task.FromResult(chatGroupMembers);
    }

    public List<Message> CreateChatMessages(List<Message> messages)
    {
        _dbContext.Messages.AddRange(messages);
        _dbContext.SaveChanges();
        return messages;     
    }

    public List<ChatGroup> GetChatGroups()
    {
        return _dbContext.ChatGroups.ToList();
    }

    public List<ChatGroup> GetChatGroupsByIdeaId(int ideaId)
    {
        return _dbContext.ChatGroups.Where(g => g.IdeaId == ideaId).ToList();
    }

    public List<ChatGroupMember> GetChatMembersByChatGroupId(int chatGroupId)
    {
        return _dbContext.ChatGroupMembers.Where(i => i.ChatGroupId == chatGroupId).ToList();
    }

    public List<Message> GetMessagesByChatGroupId(int chatGroupId)
    {
        return _dbContext.Messages.Where(m => m.ChatGroupId == chatGroupId).ToList(); 
    }

}
