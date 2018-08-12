using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Recollectable.API.Models;
using Recollectable.Data.Repositories;
using Recollectable.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recollectable.API.Controllers
{
    [Route("api/collections")]
    public class CollectionsController : Controller
    {
        private ICollectionRepository _collectionRepository;
        private IUserRepository _userRepository;

        public CollectionsController(ICollectionRepository collectionRepository,
            IUserRepository userRepository)
        {
            _collectionRepository = collectionRepository;
            _userRepository = userRepository;
        }

        [HttpGet]
        public IActionResult GetCollections()
        {
            var collectionsFromRepo = _collectionRepository.GetCollections();
            var collections = Mapper.Map<IEnumerable<CollectionDto>>(collectionsFromRepo);
            return Ok(collections);
        }

        [HttpGet("{id}", Name = "GetCollection")]
        public IActionResult GetCollection(Guid id)
        {
            var collectionFromRepo = _collectionRepository.GetCollection(id);

            if (collectionFromRepo == null)
            {
                return NotFound();
            }

            var collection = Mapper.Map<CollectionDto>(collectionFromRepo);
            return Ok(collection);
        }

        [HttpPost]
        public IActionResult CreateCollections([FromBody] CollectionCreationDto collection)
        {
            if (collection == null)
            {
                return BadRequest();
            }

            var user = _userRepository.GetUser(collection.UserId);

            if (user == null)
            {
                return BadRequest();
            }

            var newCollection = Mapper.Map<Collection>(collection);
            newCollection.User = user;

            _collectionRepository.AddCollection(newCollection);

            if (!_collectionRepository.Save())
            {
                throw new Exception("Creating a collection failed on save.");
            }

            var returnedCollection = Mapper.Map<CollectionDto>(newCollection);
            return CreatedAtRoute("GetCollection",
                new { id = returnedCollection.Id },
                returnedCollection);
        }

        [HttpPost("{id}")]
        public IActionResult BlockCollectionCreation(Guid id)
        {
            if (_collectionRepository.CollectionExists(id))
            {
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }

            return NotFound();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateCollection(Guid id, [FromBody] CollectionUpdateDto collection)
        {
            if (collection == null)
            {
                return BadRequest();
            }

            if (!_userRepository.UserExists(collection.UserId))
            {
                return BadRequest();
            }

            var collectionFromRepo = _collectionRepository.GetCollection(id);

            if (collectionFromRepo == null)
            {
                return NotFound();
            }

            collectionFromRepo.UserId = collection.UserId;

            Mapper.Map(collection, collectionFromRepo);
            _collectionRepository.UpdateCollection(collectionFromRepo);

            if (!_collectionRepository.Save())
            {
                throw new Exception($"Updating collection {id} failed on save.");
            }

            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult PartiallyUpdateCollection(Guid id,
            [FromBody] JsonPatchDocument<CollectionUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var collectionFromRepo = _collectionRepository.GetCollection(id);

            if (collectionFromRepo == null)
            {
                return NotFound();
            }

            var patchedCollection = Mapper.Map<CollectionUpdateDto>(collectionFromRepo);
            patchDoc.ApplyTo(patchedCollection);

            if (!_userRepository.UserExists(patchedCollection.UserId))
            {
                return BadRequest();
            }

            collectionFromRepo.UserId = patchedCollection.UserId;

            Mapper.Map(patchedCollection, collectionFromRepo);
            _collectionRepository.UpdateCollection(collectionFromRepo);

            if (!_collectionRepository.Save())
            {
                throw new Exception($"Patching collection {id} failed on save.");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCollection(Guid id)
        {
            var collectionFromRepo = _collectionRepository.GetCollection(id);

            if (collectionFromRepo == null)
            {
                return NotFound();
            }

            _collectionRepository.DeleteCollection(collectionFromRepo);

            if (!_collectionRepository.Save())
            {
                throw new Exception($"Deleting collection {id} failed on save.");
            }

            return NoContent();
        }
    }
}