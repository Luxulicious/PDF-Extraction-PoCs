﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toxy.Test
{
    [TestFixture]
    public class EmlEmailParserTest
    {

        [Test]
        public void ReadEmlTest()
        {
            string path = TestDataSample.GetFilePath("test.eml",null);
            ParserContext context = new ParserContext(path);
            IEmailParser parser = ParserFactory.CreateEmail(context);
            ToxyEmail email = parser.Parse();
            Assert.AreEqual(1, email.From.Count);
            Assert.AreEqual(1, email.To.Count);
            Assert.AreEqual("=?utf-8?B?5ouJ5Yu+572R?= <service@email.lagou.com>", email.From[0]);
            Assert.AreEqual("tonyqus@163.com", email.To[0]);

            Assert.IsTrue(email.Subject.StartsWith("=?utf-8?B?5LiK5rW35YiG5LyX5b635bOw5bm/5ZGK?= =?utf-8?B?5Lyg5p"));
            Assert.IsTrue(email.Body.StartsWith("------=_Part_4546_1557510524.1418357602217\r\nContent-Type: text"));
            Assert.IsNull(email.HtmlBody);

        }
    }
}
