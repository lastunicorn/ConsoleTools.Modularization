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

internal sealed class TimeBox : IDisposable, IAsyncDisposable
{
    private readonly Timer timer;
    private volatile bool timeElapsed;

    public TimeSpan MaxExecutionTime { get; set; } = TimeSpan.FromSeconds(1);

    public Action OnTimeElapsed { get; set; }

    public TimeBox()
    {
        timer = new Timer(HandleTimerElapsed);
    }

    private void HandleTimerElapsed(object state)
    {
        StopTimer();
        timeElapsed = true;

        OnTimeElapsed?.Invoke();
    }

    public void Run(Action action)
    {
        StartTimer();

        try
        {
            action?.Invoke();

            if (timeElapsed)
                throw new Exception($"Allocated time for execution elapsed: {MaxExecutionTime.Milliseconds:N0} ms.");
        }
        finally
        {
            StopTimer();
        }
    }

    public void Stop()
    {
        StopTimer();
    }

    private void StartTimer()
    {
        timer.Change(MaxExecutionTime, TimeSpan.FromTicks(-1));
    }

    private void StopTimer()
    {
        timer.Change(-1, -1);
    }

    public void Dispose()
    {
        timer?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (timer != null) await timer.DisposeAsync();
    }
}