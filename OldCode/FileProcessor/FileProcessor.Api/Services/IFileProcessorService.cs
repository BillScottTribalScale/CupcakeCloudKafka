using FileProcessor.Api.Models;

namespace FileProcessor.Api.Services {

    public interface IFileProcessorService {

        ProcessSummary StartProcess (string fileContent);
        
    }

}