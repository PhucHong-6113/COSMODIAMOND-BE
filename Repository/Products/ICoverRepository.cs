﻿using Microsoft.EntityFrameworkCore;
using Model.Models;

namespace Repository.Products
{
    public interface ICoverRepository
    {
        IEnumerable<Cover> GetAllCovers();
        Cover GetCoverById(int coverId);
        void AddCover(Cover cover);
        void UpdateCover(Cover cover);
        void DeleteCover(int coverId);
        void Save();
        void DetachLocalCover(Cover t, string entryId);
        void DetachLocalSize(CoverSize t, string entryId);
        void DetachLocalMetalType(CoverMetaltype t, string entryId);
        void EmptyCover(int id);
    }
}
