using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;

using PresetStore.Data;
using RITS.StrymonEditor.Models;

namespace PresetStore.Controllers
{
    [ODataRoutePrefix("Presets")]
    public class PresetController : ODataController
    {
        private readonly PresetStoreContext _context = new PresetStoreContext();

        [EnableQuery]
        [ODataRoute]
        public IHttpActionResult GetFeed()
        {
            return Ok(_context.Presets);
        }
        [ODataRoute("({id})")]
        [EnableQuery]
        public IHttpActionResult GetEntity(int id)
        {
            return Ok(SingleResult.Create<DBPreset>(_context.Presets.Where(t => t.PresetId == id)));
        }

        public IHttpActionResult PostSearch(ODataActionParameters parameters)
        {
            PresetSearch search = null;
            return Ok(_context.Search(search));
        }
        
    }

    [ODataRoutePrefix("Tags")]
    public class TagController : ODataController
    {
        private readonly PresetStoreContext _context = new PresetStoreContext();

        [EnableQuery]
        [ODataRoute]
        public IHttpActionResult GetFeed()
        {
            return Ok(_context.Tags);
        }
        [ODataRoute("({id})")]
        [EnableQuery]
        public IHttpActionResult GetEntity(int id)
        {
            return Ok(SingleResult.Create<DBTag>(_context.Tags.Where(t => t.TagId == id)));
        }


    }

    [ODataRoutePrefix("Parameters")]
    public class ParametersController : ODataController
    {
        private readonly PresetStoreContext _context = new PresetStoreContext();

        [EnableQuery]
        [ODataRoute]
        public IHttpActionResult GetFeed()
        {
            return Ok(_context.Parameters);
        }
        [ODataRoute("({id})")]
        [EnableQuery]
        public IHttpActionResult GetEntity(int id)
        {
            return Ok(SingleResult.Create<DBParameter>(_context.Parameters.Where(t => t.ParameterId == id)));
        }
    }

    [ODataRoutePrefix("PresetTags")]
    public class PresetTagsController : ODataController
    {
        private readonly PresetStoreContext _context = new PresetStoreContext();

        [EnableQuery]
        [ODataRoute]
        public IHttpActionResult GetFeed()
        {
            return Ok(_context.PresetTags);
        }
        [ODataRoute("({id})")]
        [EnableQuery]
        public IHttpActionResult GetEntity(int id)
        {
            return Ok(SingleResult.Create<DBPresetTag>(_context.PresetTags.Where(t => t.PresetTagId == id)));
        }
    }


    public class UnboundController : ODataController
    {
        private readonly PresetStoreContext _context = new PresetStoreContext();
        [HttpGet]
        [ODataRoute("GetSalesTaxRate(state={state})")]
        public IHttpActionResult GetSalesTaxRate(string state)
        {
            return Ok(12);
        }

        [HttpPost]
        [ODataRoute("SearchForPresets")]
        public IHttpActionResult SearchForPresets(ODataActionParameters parameters)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            PresetSearch  criteria = (PresetSearch)parameters["criteria"];
            return Ok(_context.Search(criteria));
        }

        [HttpPost]
        [ODataRoute("UploadPreset")]
        public IHttpActionResult UploadPreset(ODataActionParameters parameters)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            StrymonXmlPreset preset = (StrymonXmlPreset)parameters["uploadPreset"];
            _context.UploadPreset(preset);
            return Ok();
        }
    }
}