using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Cambridge.Demo.ResourceServer.Controllers
{
	//The Authorize Attribute will require to have a valid Access Token without additional policy
	[Authorize]
	[Route("[controller]/[action]/{name}")]
	public class DocumentController : ControllerBase
    {
	    readonly IAuthorizationService _authorizationService;

	    public DocumentController(IAuthorizationService authorizationService)
	    {
		    _authorizationService = authorizationService;
	    }

		[HttpGet]
		[Route("[controller]/[action]")]
	    public IActionResult Client()
	    {
		    string clientResource = "This is a client Document";
		    return Ok(clientResource);
	    }

		[HttpGet]
	    public async Task<IActionResult> GetFree(string name)
	    {
		    var auth = await _authorizationService.AuthorizeAsync(User, "ScopedPolicy");
		    if (auth.Succeeded)
		    {
			    var result = DocumentRepository.FreeDocuments.SingleOrDefault(x => x.Name.Equals(name));

			    if (result is null)
				    return NotFound();
			    return Ok(result.Content);
		    }
		    return Forbid();
		}

		[HttpGet]
		public async Task<IActionResult> GetPersonal(string name)
	    {
		    var result = DocumentRepository.PersonalDocuments.SingleOrDefault(x => x.Name.Equals(name));
		    if (result is null)
			    return NotFound();

			var auth = await _authorizationService.AuthorizeAsync(User, result,"DocumentOwnerPolicy");
		    if (auth.Succeeded)
			    return Ok(result.Content);

		    return Forbid();
		}
    }
}