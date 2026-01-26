using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanQuest.Domain.Entities
{

        public class Tenant
        {
            public Guid Id { get; private set; }
            public string Name { get; private set; }
            public string ApiKey { get; private set; }
            public bool IsActive { get; private set; }
            public DateTime CreatedAt { get; private set; }

            public MpesaConfiguration? MpesaConfiguration { get; private set; }
        public string? MpesaConfigJson { get; set; }

        protected Tenant() { }

            public Tenant(string name, string apiKey)
            {
                Id = Guid.NewGuid();
                Name = name;
                ApiKey = apiKey;
                IsActive = true;
                CreatedAt = DateTime.UtcNow;
            }

            public void Activate() => IsActive = true;
            public void Deactivate() => IsActive = false;

            public void SetMpesaConfiguration(MpesaConfiguration config)
            {
                MpesaConfiguration = config;
            }
        }
    }
