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

public class ContentService : IContentService
{
    private readonly ILoggingIdeaIncubator loggingIdeaIncubator;
    private readonly IIdeaService ideaService;
    private readonly IdeaIncubatorDbContext _dbContext;

    public ContentService(IdeaIncubatorDbContext dbContext, ILoggingIdeaIncubator loggingIdeaIncubator, IIdeaService ideaService)
    {
        this.loggingIdeaIncubator = loggingIdeaIncubator;
        this.ideaService = ideaService;
        _dbContext = dbContext;
    }

    public async Task<Content> CreateContentAsync(Content content, int ideaId)
    {
        content.CreatedDate = DateTime.Now;
        _dbContext.Contents.Add(content);
        await _dbContext.SaveChangesAsync();
        await AddIdeaContent(content.ContentId, ideaId);
        return content;
    }

    private async Task<bool> AddIdeaContent(int contentId, int ideaId)
    {
        bool result = true;
        try
        {
            IdeaContent ideaContent = new IdeaContent();
            ideaContent.ContentId = contentId;
            ideaContent.IdeaId = ideaId;
            _dbContext.IdeaContents.Add(ideaContent);
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            result = false;
        }
        return result;
    }
    public async Task<bool> DeleteContent(Content content)
    {
        bool result = true;
        try
        {
            // Queue up removing the idea/content links
            _dbContext.IdeaContents.Where(ic => ic.ContentId == content.ContentId).ToList().ForEach(ic => _dbContext.IdeaContents.Remove(ic));
            // Queue up removing the content/comments links
            _dbContext.ContentComments.Where(cc => cc.ContentId == content.ContentId).ToList().ForEach(cc => _dbContext.ContentComments.Remove(cc));
            // Queue up removing content
            _dbContext.Contents.Remove(content);
            await _dbContext.SaveChangesAsync();
        } catch (Exception ex)
        {
            result = false;
        }
        return result;
    }

    public List<Content> GetAllContentsOfAnIdea(int id, int level = 0)
    {
        List<IdeaContent> ideaContents = new List<IdeaContent>();
        List<Content> contents = new List<Content>();
        try
        {
            ideaContents = _dbContext.IdeaContents.Where(ic => ic.IdeaId == id).ToList();
            foreach (IdeaContent ideaContent in ideaContents)
            {
                Content tempContent = _dbContext.Contents.FirstOrDefault(c => c.ContentId == ideaContent.ContentId);
                if (tempContent.DisclosureLevel <= level)
                {
                    contents.Add(tempContent);
                }
            }
        }
        catch (Exception ex)
        {
        }
        return contents;
    }

    public Content GetContent(int id)
    {
        Content content = null;
        content = _dbContext.Contents.FirstOrDefault(c => c.ContentId == id);
        return content;
    }

    public async Task<bool> UpdateContent(Content content)
    {
        bool result = true;
        try
        {
            _dbContext.Contents.Update(content);
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            result = false;
        }
        return result;
    }
}
