using System;
using System.Threading;
using System.Threading.Tasks;
using NSoldat.Lib;

namespace NSoldat.MultiServerClient
{
    public class SoldatTournament
    {
        private readonly TimeSpan _matchLength = TimeSpan.FromMinutes(5) + TimeSpan.FromSeconds(2);
        private readonly TimeSpan _warmupLength = TimeSpan.FromMinutes(3);

        private bool _isRunnerPaused = false;

        public void Start(ISoldatClientCommands client)
        {

            /*
             *  ctf_Ash
                ctf_Kampf
                ctf_Equinox
                ctf_Chernobyl
                ctf_Rotten
                ctf_Cobra
                ctf_IronNuts
                ctf_Viet
             */
            var plan = GetMatchPlan(client, "ctf_Equinox");
            Task.Run(() => Runner(plan));
        }

        private void Runner(Func<Task>[] steps)
        {
            int stepIndex = -1;
            Task currentTask = null;
            while (true)
            {
                if (_isRunnerPaused)
                {
                    Thread.Sleep(100);
                    continue;
                }

                if (currentTask != null && !currentTask.IsCompleted)
                {
                    Thread.Sleep(10);
                    continue;
                }
                
                if (stepIndex >= steps.Length)
                {
                    break;
                }

                stepIndex++;
                Console.WriteLine("Running step " + stepIndex);
                var currentStep = steps[stepIndex];
                currentTask = currentStep();
            }

            Console.WriteLine("Tournament finished");
        }

        private Func<Task>[] GetMatchPlan(ISoldatClientCommands client, string currentMapName)
        {
            return new Func<Task>[]
            {
                Do(() => client.SetMap(currentMapName)),

                Wait(TimeSpan.FromSeconds(10)),
                Do(() => client.Say("Warm-up session - check you server and team!")),
                Wait(_warmupLength -TimeSpan.FromSeconds(30)),

                Do(() => client.Say("Warm-up session ends in 30 seconds!")),
                Wait(TimeSpan.FromSeconds(20)),
                Countdown(client, 10),

                Do(() => client.SetMap(currentMapName)),
                Wait(TimeSpan.FromSeconds(10)),
                Do(() => client.Say("Tournament match, Round 1 - This is not a drill!")),
                Wait(_matchLength),

                Do(() => client.SetMap(currentMapName)),
                Wait(TimeSpan.FromSeconds(3)),
                Do(() => client.SwapTeams()),
                Wait(TimeSpan.FromSeconds(2)),
                Do(() => client.Say("Tournament match, Round 2, Teams Swapped - This is not a drill!")),
                Wait(_matchLength),
            };
        }

        private Func<Task> Do(Action action)
        {
            return () =>
            {
                action();
                return Task.CompletedTask;
            };
        }

        private Func<Task> Wait(TimeSpan span) => () => Task.Delay(span);

        private Func<Task> Countdown(ISoldatClientCommands client, int n)
        {
            return async () =>
            {
                for (int i = n; i > 0; --i)
                {
                    client.Say($"{i}...");
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            };
        }

        private Func<Task> WaitForUser()
        {
            return () =>
            {
                return Task.Run(() =>
                {
                    Console.WriteLine("Press any key to continue tournament");
                    Console.ReadKey();
                });
            };
        }

        public void TogglePause()
        {
            if (_isRunnerPaused)
            {
                Console.WriteLine("Resuming tournament");
            }
            else
            {
                Console.WriteLine("Pausing tournament");
            }

            _isRunnerPaused = !_isRunnerPaused;
        }
    }
}