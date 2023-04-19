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

namespace IdeaIncubatorBlazor.Utils.Loggings;

public class LoggingIdeaIncubator : ILoggingIdeaIncubator
{
    private readonly ILogger logger;

    public LoggingIdeaIncubator(ILogger logger) => this.logger = logger;

    public void LogCritical(Exception exception) => this.logger.LogCritical(exception, exception.Message);

    public void LogDebug(string message) => this.logger.LogDebug(message);

    public void LogError(Exception exception) => this.logger.LogError(exception, exception.Message);

    public void LogInformation(string message) => this.logger.LogInformation(message);

    public void LogTrace(string message) => this.logger.LogTrace(message);

    public void LogWarning(string message) => this.logger.LogWarning(message);

}
