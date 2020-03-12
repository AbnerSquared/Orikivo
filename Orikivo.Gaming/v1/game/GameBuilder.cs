using System.Collections.Generic;

namespace Orikivo
{

    /// <summary>
    /// Base rulesets that define how a game is handled.
    /// </summary>
    public class GameBuilder
    {
        private GameBuilder() {}
        // learn about what is required for the game properties to generate.
        // you need the list of users in order to properly determine what attributes need to exist
        public static GameBuilder Create(GameMode mode, List<Identity> users)
        {
            GameBuilder builder = new GameBuilder();
            
            // name
            builder.Name = "Werewolf";

            // to start
            var toStart = GameLobbyCriteria.Create(5, 15);


            // global attributes
            /*
                 1. player_count: how many people are currently playing?
                 2. living_werewolf_count: how many living people are team werewolf?  
                 3. living_villager_count: how many living people are team villager?
                 4. phase_id: what phase are we currently on?
                 5. round_count: what round are we currently on? (a full cycle)

            */


            // per user attributes
            /*
                1. is_dead - is the player dead? (this prevents them from executing any commands aside from leave game, etc.)
                2. role_id - what is the role that the player has? (this determines how the night session functions)
                3. was_accused - was the player accused (this prevents them from being accused again for the duration of the day)
                4. was_scanned - was the player scanned by a seer? (this makes sure the seer can't rescan, and allows the seer to see their morality.
                
                
            */

            // role_generate: task that sets the roles for each user, and lets them read info about it.
            /*
                on entry:
                   foreach user
                        - pick role_id from rand_roles GameVarBag (a collection of various values)

             
                timeout: 20sec 
             
                transition:
                    -> open:
                        default
                    
                    -> night:
                        only if:
                            - L:skip_count == G:player_count

                local attributes:
                    - skip_count: a counter of all skips called for.
                    

                local value bags:
                    - role_ids: a collection of ids generated based on user count.

                local user attributes:
                    - skipped: did the user want to skip the intro?
             
            */


            // open
            /*
                on entry:
                    foreach player_slot
                        - pick user_index from user_indexes
                
                timeout: 25sec

                local value bags:
                    - user_indexes: a collection of the slots of all playing users.

                local attributes:
                    - player_slot_1: template slot for names
                    - player_slot_2: template slot for names
                    - player_slot_3: template slot for names
             
            */
            // day0
            /*
             
             
            */
            // night
            /*
                L:all_abilities_used: were all of the abilities used?

                commands:
             
            */
            // killed_at_night
            /*
             
             
            */
            // day
            /*
             
             
            */
            // accuse
            /*
             
             
            */
            // defense
            /*
             
             
            */
            // vote
            /*
             
             
            */
            // killed_by_vote
            /*
             
             
            */
            // result
            /*
             
             
            */

            // all possible different task
            /*
                //1. generate roles

                //    a. let people read their roles

                //2. entry opener (talk about stuff during day)

                //    a. talk about what happens during night

                //    b. talk about discovery of what happened at night, transition to suspicion

                //3 create a day0 session, where people can only talk and whatknot.

                //4. night session, everyone is sleeping
                    
                //    a. allow users with abilities to use them now.
                        
                //        i. werewolves go first, choosing who to kill. (dms)

                //        ii. seers go next, choosing who to scan.  (dms)

                //5. if anyone was killed, transition to killed_at_night phase


                //    a. killed_at_night phase shows the person that was killed
                        
                //        i. check for win conditions, and if any were met, transition to results.

                //        ii. otherwise, transition to day session.

                //6. create a day session, where people can accuse each other and chat.

                //    a. when accused, wait for anyone else to agree

                //    b. when time runs out, transition to night.

                //    c. if accused_counter = 3, transition to night.

                //7. accuse session, where someone is accused, and the game waits to see if anyone else agrees.

                //    a. if everyone says no, return to day session and tick accused_counter by 1.

                //    b. if time runs out, return to day session and tick accused_counter by 1.

                //   c. if someone agrees, transition to defense.
                
                //8. defense session, where the person accused has time to say whatever they need to defend themselves

                //    a. if time runs out, say the person took too long to comply, and transition to vote.

                //    b. if they don't want to write, say the person did not want to comply, and transition to vote.

                //    c if the person did write something, show what they wrote, and transition to vote.
                
                //9. vote session, where the person accused is put on trial, and everyone gets to choose if they live or die.

                //    a. if nobody has voted, repeat the process.

                //        i for each person that doesn't reply, their vote is ignored.

                //    c. if the votes tally up to kill, transition to kill_by_vote.

                //    d. if the votes tally up to live, transition to day and tick accused_counter by 1 and tick failed_trials by 1.
                
                //10. kill_by_vote session, where the person voted for is hung, showing their role in the process.
                    
                //    a. if any win conditions are met, transition to results.

                //    b. otherwise, transition to night.

                //11. results session, where the side that won is shown, along with random stats.

                //    a. if all users leave OR vote to close results, end the game session, and return the GameResult object.

                //    b. if the timeout occurs, end the game session, and return the GameResult object.
                
             */



            return builder;
        }

        public string Name { get; set; }
        public GameLobbyCriteria ToStart { get; set; }
        public List<GameAttribute> GlobalAttributes { get; set; }
        public List<GameAttribute> UserAttributes { get; set; }
        //public List<GameTaskProperties> Tasks { get; set; }
        public List<GameTrigger> Commands { get; set; } // GameTrigger => GameCommand
        // List of a list of attributes, with what to do on success.
        public List<GameCriterion> ToComplete { get; set; }
        public List<GameRule> Rules { get; set; }
        public GameWindowProperties WindowProperties { get; set; }
        // public GameTaskProperties EntryTask { get; set; }

        public GameTask EntryTask { get; private set; }
        public List<GameAttribute> Attributes { get; private set; } // the root list of attributes.
        public List<GameTask> Tasks { get; private set; } // the list of tasks.



        public GameTask ExitTask { get; private set; }

        public GameClientData BaseData { get; private set; }
    }

    // create a function determining what to do upon starting from a specific task.
}
