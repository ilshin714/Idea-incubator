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

public interface IIdeaService
{
    Idea GetIdea(int id);

    List<Idea> GetIdeas();

    List<Idea> SearchIdeas(string searchInput);

    Task<Idea> CreateIdeaAsync(Idea idea, int userId);

    Idea UpdateIdea(Idea idea);

    Task<string> DeleteIdea(int ideaId);
    Task<User> GetProviderOfIdea(int ideaID);
    Task<List<UserIdeaRole>> GetCollaboratorsOfIdea(int ideaID, bool incProvider = false);
    Task<List<UserIdeaRole>> GetRequestersOfIdea(int ideaID);
    List<Idea> GetOwnIdeas(int userId);
    List<Idea> GetCollabIdeas(int userId);
    List<Idea> GetWishList(int userId);
    Task<List<UserIdea>> GetUserIdeasAsync(int userId);
    UserIdea CreateUserIdea(UserIdea userIdea);
    Task<UserIdea> CreateUserIdeaAsync(UserIdea userIdea);
    UserIdea UpdateUserIdea(UserIdea userIdea);
    Task<UserIdea> UpdateUserIdeaAsync(UserIdea userIdea);
}
