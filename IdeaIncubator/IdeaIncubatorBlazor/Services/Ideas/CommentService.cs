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
using IdeaIncubatorBlazor.Utils.Loggings;
using Microsoft.EntityFrameworkCore;

namespace IdeaIncubatorBlazor.Services.Ideas;

public class CommentService : ICommentService
{
    private readonly ILoggingIdeaIncubator loggingIdeaIncubator;
    private readonly IContentService contentService;
    private readonly IdeaIncubatorDbContext _dbContext;

    public CommentService(IdeaIncubatorDbContext dbContext, ILoggingIdeaIncubator loggingIdeaIncubator, IContentService contentService)
    {
        this.loggingIdeaIncubator = loggingIdeaIncubator;
        this.contentService = contentService;
        _dbContext = dbContext;
    }

    public async Task<Comment> CreateCommentAsync(Comment comment, int contentId)
    {
        comment.DateCreated = DateTime.Now;
        _dbContext.Comments.Add(comment);
        await _dbContext.SaveChangesAsync();
        await AddContentComment(comment.CommentId, contentId);
        return comment;
    }

    private async Task<bool> AddContentComment(int commentId, int contentId)
    {
        bool result = true;
        try
        {
            ContentComment contentComment = new ContentComment();
            contentComment.ContentId = contentId;
            contentComment.CommentId = commentId;
            _dbContext.ContentComments.Add(contentComment);
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            result = false;
        }
        return result;
    }

    public List<Comment> GetAllCommentsOfAContent(int id)
    {
        List<ContentComment> contentComments = new List<ContentComment>();
        List<Comment> comments = new List<Comment>();
        try
        {
            contentComments = _dbContext.ContentComments.Where(cc => cc.ContentId == id).ToList();
            foreach (ContentComment contentComment in contentComments)
            {
                comments.Add(_dbContext.Comments.FirstOrDefault(c => c.CommentId == contentComment.CommentId));
            }
        }
        catch (Exception ex)
        {
        }
        return comments;
    }

    public Comment GetComment(int id)
    {
        Comment comment = null;
        comment = _dbContext.Comments.FirstOrDefault(c => c.CommentId == id);
        return comment;
    }

    public async Task<Comment> GetCommentAsync(int id)
    {
        Comment comment = null;
        comment = await _dbContext.Comments.FirstOrDefaultAsync(c => c.CommentId == id);
        return comment;
    }

    public async Task<bool> DeleteComment(Comment comment)
    {
        bool result = true;
        try
        {
            // Queue up removing the content/comment links
            _dbContext.ContentComments.Where(cc => cc.CommentId == comment.CommentId).ToList().ForEach(cc => _dbContext.ContentComments.Remove(cc));
            // Queue up removing comment
            _dbContext.Comments.Remove(comment);
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            result = false;
        }
        return result;
    }

    public async Task<bool> UpdateComment(Comment comment)
    {
        bool result = true;
        try
        {
            _dbContext.Comments.Update(comment);
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            result = false;
        }
        return result;
    }
}
