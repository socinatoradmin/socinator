#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.FileManagers
{
    public interface IGenericFileManager
    {
        List<T> GetModuleDetails<T>(string filePath) where T : class;

        bool UpdateModuleDetails<T>(List<T> detailsList) where T : class;

        // void SaveAll<T>(List<T> lstModel) where T : class;
        // void SaveAll<T>(List<T> lstModel, string file) where T : class;
        bool Save<T>(T model, string file) where T : class;
        T GetModel<T>(string filePath) where T : class, new();
        bool UpdateModuleDetails<T>(List<T> detailsList, string file) where T : class;

        bool AddRangeModule<T>(List<T> moduleToSave, string filePath) where T : class;

        bool AddModule<T>(T moduleToSave, string filePath) where T : class;

        void Delete<T>(Predicate<T> match, string filePath) where T : class;
        bool DeleteBinFiles(string filepath);
        bool Overrride<T>(T instance, string filePath) where T : class;
        bool UpdateAdvancedSettingDetails<T>(List<T> detailsList, string fileType) where T : class;
    }

    public class GenericFileManager : IGenericFileManager
    {
        private readonly IProtoBuffBase _protoBuffBase;
        private readonly ILockFileConfigProvider _lockFileConfigProvider;
        private readonly IFileSystemProvider _fileSystemProvider;

        public GenericFileManager(IProtoBuffBase protoBuffBase, ILockFileConfigProvider lockFileConfigProvider,
            IFileSystemProvider fileSystemProvider)
        {
            _protoBuffBase = protoBuffBase;
            _lockFileConfigProvider = lockFileConfigProvider;
            _fileSystemProvider = fileSystemProvider;
        }


        /// <summary>
        ///     To get the file details for given bin files
        /// </summary>
        /// <typeparam name="T">Target type</typeparam>
        /// <param name="filePath">file path</param>
        /// <returns></returns>
        public List<T> GetModuleDetails<T>(string filePath) where T : class
        {
            return _fileSystemProvider.Exists(filePath) ? _protoBuffBase.DeserializeList<T>(filePath) : new List<T>();
        }


        /// <summary>
        ///     To Update the details of the files
        /// </summary>
        /// <typeparam name="T">Target type</typeparam>
        /// <param name="detailsList">List of file details</param>
        /// <returns></returns>
        public bool UpdateModuleDetails<T>(List<T> detailsList) where T : class
        {
            try
            {
                // Fetch the file path from lock with type object
                return _lockFileConfigProvider.WithFile<T, bool>(file =>
                {
                    // serialize the file
                    var result = _protoBuffBase.SerializeList(detailsList, file);
                    return result;
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }
        }


        ///// <summary>
        ///// To save all the details of the give object to respective file type's objects
        ///// </summary>
        ///// <typeparam name="T">Target type</typeparam>
        ///// <param name="lstModel">List of objects which is present after saved in bin file</param>
        //internal static void SaveAll<T>(List<T> lstModel) where T : class
        //{
        //    UpdateModuleDetails(lstModel);
        //    GlobusLogHelper.log.Debug("Details successfully saved");
        //}

        ///// <summary>
        ///// To save the details with given file 
        ///// </summary>
        ///// <typeparam name="T">Targer type</typeparam>
        ///// <param name="lstModel">List of details</param>
        ///// <param name="file">saving file path</param>
        //internal static void SaveAll<T>(List<T> lstModel, string file) where T : class
        //{
        //    UpdateModuleDetails(lstModel, file);
        //    GlobusLogHelper.log.Debug("Details successfully saved");
        //}


        /// <summary>
        ///     Save the details to specified file
        /// </summary>
        /// <typeparam name="T">Target type</typeparam>
        /// <param name="model">Details</param>
        /// <param name="file">file path</param>
        /// <returns></returns>
        public bool Save<T>(T model, string file) where T : class
        {
            try
            {
                using (var stream = _fileSystemProvider.Create(file))
                {
                    // Call for serialize
                    Serializer.Serialize(stream, model);
                    return true;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }
        }

        /// <summary>
        ///     To Fetch the details from file
        /// </summary>
        /// <typeparam name="T">Target type</typeparam>
        /// <param name="filePath">file path</param>
        /// <returns></returns>
        public T GetModel<T>(string filePath) where T : class, new()
        {
            var result = new T();
            try
            {
                if (_fileSystemProvider.Exists(filePath))
                    using (var stream = _fileSystemProvider.OpenReadonly(filePath))
                    {
                        // Call for deserialize
                        result = Serializer.Deserialize<T>(stream);
                    }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return result;
        }

        /// <summary>
        ///     To Update the details to specified files
        /// </summary>
        /// <typeparam name="T">Targer Type</typeparam>
        /// <param name="detailsList">Detail list</param>
        /// <param name="file">file path</param>
        /// <returns></returns>
        public static readonly Mutex mutex = new Mutex(false, "UpdateModuleDetails");
        public bool UpdateModuleDetails<T>(List<T> detailsList, string file) where T : class
        {
            var result = false;
            mutex.WaitOne();
            try
            {
                // Call for serialize
                result = _protoBuffBase.SerializeList(detailsList, file);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            finally
            {
                mutex.ReleaseMutex();
            }
            return result;
        }


        /// <summary>
        ///     Add multiple type values to file
        /// </summary>
        /// <typeparam name="T">Targer Type</typeparam>
        /// <param name="moduleToSave">type values</param>
        /// <param name="filePath">file path</param>
        /// <returns></returns>
        public bool AddRangeModule<T>(List<T> moduleToSave, string filePath) where T : class
        {
            try
            {
                moduleToSave.ForEach(x =>
                {
                    // Call for append the details 
                    _protoBuffBase.AppendObject(x, filePath);
                });
                return true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }
        }

        /// <summary>
        ///     To add the details to file path
        /// </summary>
        /// <typeparam name="T">Target type</typeparam>
        /// <param name="moduleToSave">Module Details</param>
        /// <param name="filePath">file path</param>
        /// <returns></returns>
        public bool AddModule<T>(T moduleToSave, string filePath) where T : class
        {
            try
            {
                //Call for Append
                _protoBuffBase.AppendObject(moduleToSave, filePath);
                return true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }
        }

        /// <summary>
        ///     To delete the details which matches the predicate condition
        /// </summary>
        /// <typeparam name="T">target type</typeparam>
        /// <param name="match">match condition</param>
        /// <param name="filePath">file path</param>
        public void Delete<T>(Predicate<T> match, string filePath) where T : class
        {
            // Get all the details from file
            var moduleDetails = GetModuleDetails<T>(filePath);
            // Remove all matches
            moduleDetails.RemoveAll(match);
            // Update the bin files
            UpdateModuleDetails(moduleDetails, filePath);
        }

        /// <summary>
        ///     To delete the bin file
        /// </summary>
        /// <param name="filepath">file path</param>
        /// <returns></returns>
        public bool DeleteBinFiles(string filepath)
        {
            try
            {
                // Check whether file is there or not, if present delete
                if (File.Exists(filepath))
                    File.Delete(filepath);
            }
            catch (IOException ex)
            {
                ex.DebugLog();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return !File.Exists(filepath);
        }

        /// <summary>
        ///     To override the details with specified file
        /// </summary>
        /// <typeparam name="T">Target type</typeparam>
        /// <param name="instance">details</param>
        /// <param name="filePath">file path</param>
        /// <returns></returns>
        public bool Overrride<T>(T instance, string filePath) where T : class
        {
            try
            {
                using (var stream = File.Create(filePath))
                {
                    // Call for serialize
                    Serializer.Serialize(stream, instance);
                    return true;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }
        }

        public bool UpdateAdvancedSettingDetails<T>(List<T> detailsList, string fileType) where T : class
        {
            try
            {
                var result = _protoBuffBase.SerializeList(detailsList, fileType);
                return result;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }
        }
    }
}