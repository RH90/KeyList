﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyList
{
    public class Computer
    {
        private string brand, model, serielnumber, comment, smartwater, historyShort;
        private int id, buy_out, status, owner_id;

        public Computer(int id, string brand, string model, string serielnumber, string comment, string smartwater, int buy_out, int status, int owner_id, string historyShort)
        {
            this.id = id;
            this.brand = brand;
            this.model = model;
            this.serielnumber = serielnumber;
            this.comment = comment;
            this.smartwater = smartwater;
            this.buy_out = buy_out;
            this.status = status;
            this.owner_id = owner_id;
            this.historyShort = historyShort;
        }

        public string Brand { get => brand; set => brand = value; }
        public string Model { get => model; set => model = value; }
        public string Serielnumber { get => serielnumber; set => serielnumber = value; }
        public string Comment { get => comment; set => comment = value; }
        public string Smartwater { get => smartwater; set => smartwater = value; }
        public int Buy_out { get => buy_out; set => buy_out = value; }
        public int Status { get => status; set => status = value; }
        public int Owner_id { get => owner_id; set => owner_id = value; }
        public int Id { get => id; set => id = value; }
        public string HistoryShort { get => historyShort; set => historyShort = value; }
    }
}
