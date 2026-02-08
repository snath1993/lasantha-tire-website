// Template-based content generator (FREE - no API costs!)
const sql = require('mssql');
const sqlConfig = require('../sqlConfig');

class ContentGenerator {
  constructor() {
    this.useSql = String(process.env.POSTS_USE_SQL || '0') === '1';
    this.templates = {
      dailyDeal: [
        `🎉 අද දින විශේෂ දීමනාව! 🎉\n\n{brand} {product}\nප්‍රමාණය: {size}\n\n💰 විශේෂ මිල: Rs. {salePrice}\n(සාමාන්‍ය: Rs. {regularPrice})\n\n✅ ඉතිරි කරගන්න: Rs. {savings}\n📦 තොගය: {stock} units\n\nදැන් අමතන්න!\n☎️ 077-777-7777\n\n#LasanthaTyre #TyreDeals #SriLanka`,
        
        `⭐ අද ඔබට විශේෂයි! ⭐\n\n{brand} {product}\n{size}\n\n💰 අද පමණක්: Rs. {salePrice}\n(Save Rs. {savings}!)\n\n📦 Stock: {stock} units\n☎️ 077-777-7777\n\n#SpecialOffer #LasanthaTyre`,
        
        `🔥 HOT DEAL OF THE DAY! 🔥\n\n{brand} {product}\nSize: {size}\n\n💵 TODAY: Rs. {salePrice}\n📊 Regular: Rs. {regularPrice}\n\n📦 Limited stock: {stock} units\n⚡ අද අමතන්න!\n\n077-777-7777\n\n#DailyDeal #LasanthaTyre #BestPrice`
      ],
      
      tipsAndAdvice: [
        `💡 Tyre Care Tip #{tipNumber}\n\n{tipTitle}\n\n{tipDescription}\n\nඔබේ tyres දිගු කල් පවත්වාගන්න!\n\n📞 077-777-7777\n#TyreCare #LasanthaTyre`,
        
        `🔧 අද දින උපදෙස!\n\n{tipTitle}\n\n{tipDescription}\n\nසුරක්ෂිත ගමනක් සඳහා!\n\n#SafetyFirst #LasanthaTyre`
      ],
      
      weekendOffer: [
  `🎊 සති අන්ත විශේෂ දීමනාව! 🎊\n\nසියලු {category} tyres සඳහා\n\n💰 {discount}% OFF!\n\nමෙම සති අන්තයේ පමණක්!\n\n☎️ {phone}\n📍 {storeName}\n\n#WeekendSale #LasanthaTyre`,
        
  `🌟 Weekend Special 🌟\n\n{category} Tyres\n{discount}% Discount!\n\nThis weekend only!\n\nCall: {phone}\n\n#WeekendOffer #LasanthaTyre`
      ]
    };
    
    this.tips = [
      {
        title: "නිතර Air Pressure Check කරන්න",
        description: "මාසිකව air pressure check කරන්න. නිවැරදි pressure එක fuel efficiency වැඩි කරයි සහ tyre life දීර්ඝ කරයි."
      },
      {
        title: "Tyre Rotation කරන්න",
        description: "හැම කි.මී. 10,000-12,000 වලට tyres rotate කරන්න. සියලු tyres එකසේ wear වීමට උපකාරී වේ."
      },
      {
        title: "Tread Depth Monitor කරන්න",
        description: "Coin test භාවිතයෙන් tread depth පරීක්ෂා කරන්න. 1.6mm ට අඩු නම් tyres වහාම මාරු කරන්න."
      },
      {
        title: "Over-loading වළකින්න",
        description: "Vehicle load capacity ඉක්මවන්න එපා. අධික බර tyres වේගයෙන් හානිවට පත් කරයි."
      },
      {
        title: "Wheel Alignment Check කරන්න",
        description: "අසාමාන්‍ය wear patterns දැක්කොත් wheel alignment check කරන්න. Tyre ආයු කාලය දෙගුණ කළ හැකියි."
      }
    ];
  }
  
  generateProductPost(productData, templateType = 'dailyDeal') {
    const templates = this.templates[templateType];
    const template = templates[Math.floor(Math.random() * templates.length)];
    
    const savings = productData.regularPrice - productData.salePrice;
  const phone = process.env.STORE_PHONE || '0721222509';
    const storeName = process.env.STORE_NAME || 'Lasantha Tyre Traders';
    
    return template
      .replace(/{brand}/g, productData.brand || 'Quality')
      .replace(/{product}/g, productData.name)
      .replace(/{size}/g, productData.size)
      .replace(/{regularPrice}/g, productData.regularPrice.toLocaleString())
      .replace(/{salePrice}/g, productData.salePrice.toLocaleString())
      .replace(/{savings}/g, savings.toLocaleString())
      .replace(/{stock}/g, productData.stock)
      .replace(/{category}/g, productData.category || 'All')
      .replace(/{phone}/g, phone)
      .replace(/{storeName}/g, storeName);
  }
  
  generateTipPost() {
    const tip = this.tips[Math.floor(Math.random() * this.tips.length)];
    const templates = this.templates.tipsAndAdvice;
    const template = templates[Math.floor(Math.random() * templates.length)];
    
    return template
      .replace('{tipNumber}', Math.floor(Math.random() * 50) + 1)
      .replace('{tipTitle}', tip.title)
      .replace('{tipDescription}', tip.description);
  }
  
  async getRandomProduct() {
    if (!this.useSql) {
      return this.getDummyProduct();
    }
    try {
      await sql.connect(sqlConfig);
      
      const result = await sql.query`
        SELECT TOP 1 
          ItemName as name,
          Size as size,
          Brand as brand,
          Price as regularPrice,
          CAST(Price * 0.85 AS INT) as salePrice,
          StockQty as stock,
          'Passenger Car' as category
        FROM tblItemMaster
        WHERE StockQty > 10
          AND IsActive = 1
        ORDER BY NEWID()
      `;
      
      await sql.close();
      
      if (result.recordset && result.recordset.length > 0) {
        return result.recordset[0];
      }
      
      // Fallback dummy data
      return this.getDummyProduct();
      
    } catch (err) {
      console.error('SQL Error:', err.message);
      return this.getDummyProduct();
    }
  }
  
  getDummyProduct() {
    return {
      name: 'Ecopia EP150',
      size: '195/65R15',
      brand: 'Bridgestone',
      regularPrice: 18500,
      salePrice: 16500,
      stock: 25,
      category: 'Passenger Car'
    };
  }
}

module.exports = ContentGenerator;
