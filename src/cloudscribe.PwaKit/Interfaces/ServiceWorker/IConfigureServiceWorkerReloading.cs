﻿using Microsoft.AspNetCore.Http;
using System.Text;
using System.Threading.Tasks;

namespace cloudscribe.PwaKit.Interfaces
{
    public interface IConfigureServiceWorkerReloading
    {
        Task AppendToServiceWorkerScript(StringBuilder sw, PwaOptions options, HttpContext context);
    }
    
}