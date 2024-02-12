using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PotyogosAmoba.Model.Persistance
{
    public class PotyogosAmobaFileDataAccess : IPotyogosAmobaDataAccess
    {
        public async Task<PotyogosAmobaTable> LoadAsync(String path)
        {
            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    String line = await reader.ReadLineAsync() ?? String.Empty;
                    String[] fields = line.Split(' ');
                    Int32 tableSize = Int32.Parse(fields[0]);
                    PotyogosAmobaTable table = new PotyogosAmobaTable(tableSize);
                    table.CurrentPlayer = (PotyogosAmobaTable.Player)Int32.Parse(fields[1]);
                    table.OTime = new TimeSpan(0, 0, Int32.Parse(fields[2]));
                    table.XTime = new TimeSpan(0, 0, Int32.Parse(fields[3]));
                    table.StepNumber = Int32.Parse(fields[4]);

                    for (int i=0; i< tableSize; ++i)
                    {
                        line = await reader.ReadLineAsync() ?? String.Empty;
                        fields = line.Split(' ');

                        for(Int32 j = 0; j < tableSize; ++j)
                        {
                            table.Table[i, j] = (PotyogosAmobaTable.Player)Int32.Parse(fields[j]);
                        }
                    }
                    return table;
                }
            }
            catch
            {
                throw new PotyogosAmobaDataException();
            }
        }

        public async Task SaveAsync(String path, PotyogosAmobaTable table)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(path))
                {
                    writer.Write(table.Size+" ");
                    writer.Write((Int32)(table.CurrentPlayer) + " ");
                    writer.Write((Int32)(table.OTime.TotalSeconds) + " ");
                    writer.Write((Int32)(table.XTime.TotalSeconds) + " ");
                    writer.Write(table.StepNumber + " ");
                    await writer.WriteLineAsync();
                    for (Int32 i = 0; i < table.Size; i++)
                    {
                        for (Int32 j = 0; j < table.Size; j++)
                        {
                            await writer.WriteAsync((Int32)table.Table[i, j] + " "); // kiírjuk az értékeket
                        }
                        await writer.WriteLineAsync();
                    }
                }
            }
            catch
            {
                throw new PotyogosAmobaDataException();

            }
        }
    }
}
