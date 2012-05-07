using System;
using System.Linq;
using Caliburn.Micro;
using TrainShareApp.Model;

namespace TrainShareApp.Data
{
    public abstract class TokenClientBase
    {
        private readonly string _network;
        protected readonly IEventAggregator Events;
        protected readonly Func<DbDataContext> ContextFactory;

        protected TokenClientBase(string network, IEventAggregator events, Func<DbDataContext> contextFactory)
        {
            _network = network;
            Events = events;
            ContextFactory = contextFactory;

            Events.Subscribe(this);
        }

        public Token Token { get; private set; }

        public bool IsLoggedIn
        {
            get { return Token != null; }
        }

        protected void ReloadToken()
        {
            using (var context = ContextFactory())
            {
                Token = context.Tokens.FirstOrDefault(t => t.Network == _network);
            }
        }

        protected void InsertOrUpdateToken(Token token)
        {
            using (var context = ContextFactory())
            {
                context.ObjectTrackingEnabled = true;

                var original = context.Tokens.FirstOrDefault(t => t.Network == _network);

                if (original == null)
                    context.Tokens.InsertOnSubmit(token);
                else
                {
                    context.Tokens.DeleteOnSubmit(original);
                    context.Tokens.InsertOnSubmit(token);
                }

                context.SubmitChanges();
            }

            Token = token;
        }

        protected void DeleteToken()
        {
            using (var context = ContextFactory())
            {
                context.ObjectTrackingEnabled = true;

                var original = context.Tokens.FirstOrDefault(t => t.Network == _network);

                if (original == null)
                    return;
                
                context.Tokens.DeleteOnSubmit(original);
                context.SubmitChanges();
            }

            Token = null;
        }
    }
}