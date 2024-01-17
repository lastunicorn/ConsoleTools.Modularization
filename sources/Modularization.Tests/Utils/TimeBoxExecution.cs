// Console Tools Modularization
// Copyright (C) 2024 Dust in the Wind
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

namespace DustInTheWind.ConsoleTools.Modularization.Tests.Utils;

internal class TimeBoxExecution : IDisposable, IAsyncDisposable
{
    private readonly TimeBox timeBox = new();

    private TimeBoxExecution()
    {
    }

    public static TimeBoxExecution CreateNew(TimeSpan maxExecutionTime)
    {
        TimeBoxExecution timeBoxExecution = new();
        timeBoxExecution.timeBox.MaxExecutionTime = maxExecutionTime;
        return timeBoxExecution;
    }

    public static TimeBoxExecution CreateNew(int maxExecutionTime)
    {
        TimeBoxExecution timeBoxExecution = new();
        timeBoxExecution.timeBox.MaxExecutionTime = TimeSpan.FromMilliseconds(maxExecutionTime);
        return timeBoxExecution;
    }

    public TimeBoxExecution OnElapsedTime(Action action)
    {
        timeBox.OnTimeElapsed = action;
        return this;
    }

    public void Run(Action action)
    {
        try
        {
            timeBox.Run(action);
        }
        finally
        {
            timeBox.Dispose();
        }
    }

    public void Dispose()
    {
        timeBox?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (timeBox != null) await timeBox.DisposeAsync();
    }
}