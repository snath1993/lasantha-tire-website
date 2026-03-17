import { NextRequest, NextResponse } from 'next/server';
import { getSessionIdFromRequest, getSession } from '@/core/lib/session';

const BOT_API_URL = process.env.BOT_API_URL || 'http://127.0.0.1:8585';

/**
 * POST /api/erp/reorder/send
 * Sends brand-wise re-order messages to the "Re Order" WhatsApp group.
 * Body: { messages: [{ brand: string, message: string }] }
 */
export async function POST(request: NextRequest) {
  try {
    // Auth check
    const sessionId = getSessionIdFromRequest(request);
    if (!sessionId || !getSession(sessionId)) {
      return NextResponse.json({ success: false, error: 'Unauthorized' }, { status: 401 });
    }

    const body = await request.json();
    const { messages } = body as { messages: { brand: string; message: string }[] };

    if (!messages || !Array.isArray(messages) || messages.length === 0) {
      return NextResponse.json({ success: false, error: 'messages array required' }, { status: 400 });
    }

    // 1. Find "Re Order" group
    let groupChatId: string | null = null;
    try {
      const chatsRes = await fetch(`${BOT_API_URL}/whatsapp/chats`, {
        headers: { 'Content-Type': 'application/json' },
      });
      if (chatsRes.ok) {
        const chatsData = await chatsRes.json();
        const chats = chatsData.chats || chatsData || [];
        const reOrderGroup = chats.find(
          (c: any) => c.isGroup && c.name && c.name.toLowerCase() === 're order'
        );
        if (reOrderGroup) {
          groupChatId = reOrderGroup.id?._serialized || reOrderGroup.id;
        }
      }
    } catch (e) {
      console.error('[ReOrder Send] Failed to fetch chats:', e);
    }

    if (!groupChatId) {
      return NextResponse.json({
        success: false,
        error: 'WhatsApp group "Re Order" not found. Make sure the bot is connected and the group exists.',
      }, { status: 404 });
    }

    // 2. Send each brand message with a small delay
    const results: { brand: string; success: boolean; error?: string }[] = [];

    for (const { brand, message } of messages) {
      try {
        const sendRes = await fetch(`${BOT_API_URL}/whatsapp/messages/send`, {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ chatId: groupChatId, message }),
        });

        if (sendRes.ok) {
          results.push({ brand, success: true });
        } else {
          const err = await sendRes.text();
          results.push({ brand, success: false, error: err });
        }

        // Small delay between messages to avoid rate limiting
        if (messages.length > 1) {
          await new Promise(r => setTimeout(r, 1500));
        }
      } catch (e: any) {
        results.push({ brand, success: false, error: e.message });
      }
    }

    const allSuccess = results.every(r => r.success);
    const sentCount = results.filter(r => r.success).length;

    return NextResponse.json({
      success: allSuccess,
      sentCount,
      totalCount: messages.length,
      results,
      groupName: 'Re Order',
    });
  } catch (error: any) {
    console.error('[ReOrder Send] Error:', error);
    return NextResponse.json({
      success: false,
      error: error.message || 'Failed to send messages',
    }, { status: 500 });
  }
}
